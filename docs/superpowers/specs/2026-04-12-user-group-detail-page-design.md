# User Group Detail Page — Design Spec

**Date:** 2026-04-12  
**Scope:** Frontend only. Backend endpoints assumed — caller implements them.

---

## Context

No dedicated user group detail page exists. User groups are currently shown inline as cards within `CommonsDetailPage`. There is no route `/user-groups/:userGroupId`, no deeplink, and no way to view governance charter, members, or active projects for a specific user group in isolation.

---

## Route

```
/user-groups/:userGroupId
```

New component: `src/BindingChaos.Web/src/features/commons/components/UserGroupDetailPage.tsx`  
Registered in `App.tsx` alongside existing routes.

---

## Data Requirements (assumed backend)

A new `GET /api/v1/usergroups/:id` endpoint returning a response that extends the existing list shape with charter detail:

```ts
{
  id: string
  commonsId: string
  name: string
  philosophy?: string
  foundedByPseudonym: string
  formedAt: Date
  memberCount: number
  joinPolicy: 'Open' | 'InviteOnly' | 'Approval'
  isMember: boolean           // requires auth context; false for anonymous
  charter: {
    membershipRules: {
      joinPolicy: string
      maxMembers?: number
      entryRequirements?: string
      memberListPublic: boolean
      approvalSettings?: {
        approvalThreshold: number
        vetoEnabled: boolean
      }
    }
    contestationRules: {
      resolutionWindow: string   // ISO duration or human string
      rejectionThreshold: number // 0–1 fraction
    }
    shunningRules: {
      approvalThreshold: number  // 0–1 fraction
    }
  }
}
```

Members list: `GET /api/v1/usergroups/:id/members` — paginated, returns `{ pseudonym: string }[]`.  
Active projects: existing `GET /api/v1/usergroups/:userGroupId/projects` with an `activeOnly=true` filter (or status filter).

---

## Page Sections

### 1. Header

```
← Back to [Commons Name]

[Group Name]                               [Join] / [Member ✓]
[philosophy]

Founded by [pseudonym] · [date] · [N] members · [JoinPolicy badge]
```

**Join button states:**

| Condition | UI |
|---|---|
| `joinPolicy = Open`, not member, authenticated | "Join" button |
| `joinPolicy = Approval`, not member, authenticated | "Request to Join" button |
| `joinPolicy = InviteOnly`, not member | No button; badge reads "Invite Only" |
| `isMember = true` | Inert "Member" badge; no button |
| Unauthenticated + Open or Approval | `AuthRequiredButton` wrapping the button |

Back link navigates to `/commons/:commonsId`.

---

### 2. Governance Charter (collapsible card, collapsed by default)

Collapsed by default. User expands to read.

Three sub-sections rendered as labelled key-value pairs:

**Membership Rules**
- Join policy
- Max members (omit if null)
- Entry requirements (omit if null)
- Member list: "Public" or "Private"
- Approval threshold + veto (omit if approvalSettings null)

**Contestation Rules**
- Resolution window
- Rejection threshold — rendered as percentage (e.g. `0.3` → "30%")

**Shunning Rules**
- Approval threshold — rendered as percentage

All fields read-only. Null/missing fields silently omitted.

---

### 3. Members (conditional)

Rendered only when `charter.membershipRules.memberListPublic = true`. Entirely absent otherwise — no "members are private" message.

- Heading: "Members (N)"
- Each entry: clickable pseudonym → `/profiles/:pseudonym`
- Paginated via load-more button
- Page size: 20

---

### 4. Active Projects

```
Active Projects                         [View all projects →]

[Title]     [N active amendments]    [Contested]?
[description snippet, 2 lines max]
```

- Filters to active projects via `activeOnly=true` (or equivalent) query param
- Each row links to `/projects/:projectId`
- "View all projects →" links to `/user-groups/:userGroupId/projects`
- Contested badge shown when `contestedAmendmentCount > 0`
- Load-more pattern; page size: 10

---

## Component Structure

```
UserGroupDetailPage
├── useUserGroup(userGroupId)           — fetches detail response
├── Header
│   └── JoinButton                      — encapsulates all join states
├── GovernanceCard (collapsible)
│   ├── MembershipRulesSection
│   ├── ContestationRulesSection
│   └── ShunningRulesSection
├── MembersSection (conditional)
│   └── useUserGroupMembers(userGroupId)
└── ActiveProjectsSection
    └── useProjectsForUserGroup(userGroupId, { activeOnly: true })
```

Hooks follow the existing pattern (`useProject`, `useCommons`, etc.) in `src/BindingChaos.Web/src/shared/hooks/`.

---

## Error & Loading States

- Loading: skeleton cards matching section heights (consistent with `ProjectDetailPage` pattern)
- 404 / no data: single card with message + back link
- Members load-more failure: inline error with retry, does not affect rest of page
- Projects load-more failure: same

---

## Navigation Changes

- `CommonsDetailPage` > `UserGroupCard`: group name becomes a link to `/user-groups/:userGroupId` (currently no link — inline card only)
- `ProfilePage` > `UserGroupsTab`: clicking a group navigates to `/user-groups/:userGroupId` (currently navigates to `/commons/:commonsId`)

---

## Out of Scope

- Member management (kick, invite, approve join requests)
- Editing charter or group details
- Join request status tracking (pending approval)
- Discourse thread for the user group
