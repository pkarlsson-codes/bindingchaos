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

**Registration in `App.tsx`:** the new route must be registered *before* the existing `/user-groups/:userGroupId/projects` route. React Router matches routes top-to-bottom; if the detail route comes after, the segment "projects" will be matched as the `userGroupId` param.

```tsx
<Route path="/user-groups/:userGroupId" element={<UserGroupDetailPage />} />
<Route path="/user-groups/:userGroupId/projects" element={<ProjectsByUserGroupPage />} />
```

---

## Backend Requirements

Three endpoints are required. None exist today.

### 1. `GET /api/v1/usergroups/:id` — user group detail

Returns the full detail response including charter. The `commonsName` field must be included so the frontend does not need a separate commons fetch for the back link.

`isMember` is auth-context-aware: returns `true` only when the caller is an authenticated member of this group; returns `false` for anonymous callers and non-members.

`joinPolicy` at the top level and inside `charter.membershipRules` uses the same `UserGroupJoinPolicyDto` enum already defined in the generated client (`Open | InviteOnly | Approval`).

```ts
{
  id: string
  commonsId: string
  commonsName: string            // included to avoid a separate commons fetch
  name: string
  philosophy?: string
  foundedByPseudonym: string
  formedAt: Date
  memberCount: number
  joinPolicy: UserGroupJoinPolicyDto   // Open | InviteOnly | Approval
  isMember: boolean                    // auth-context-aware; false for anonymous
  charter: {
    membershipRules: {
      joinPolicy: UserGroupJoinPolicyDto
      maxMembers?: number
      entryRequirements?: string
      memberListPublic: boolean
      approvalSettings?: {
        approvalThreshold: number
        vetoEnabled: boolean
      }
    }
    contestationRules: {
      resolutionWindow: string   // e.g. "7 days"
      rejectionThreshold: number // 0–1 fraction
    }
    shunningRules: {
      approvalThreshold: number  // 0–1 fraction
    }
  }
}
```

### 2. `GET /api/v1/usergroups/:id/members` — paginated member list

Only callable (and only returns data) when `memberListPublic = true`. Returns paginated pseudonyms.

```ts
PaginatedResponse<{ pseudonym: string }>
```

### 3. `GET /api/v1/usergroups/:id/projects` with `activeOnly=true` filter

The existing projects endpoint has no `activeOnly` param today. It must be added. The frontend implementation will use this filter; without it, all projects (including completed/archived) are shown, which is incorrect.

---

## Page Sections

### 1. Header

```
← Back to [Commons Name]

[Group Name]                               [Join] / [Member ✓]
[philosophy]

Founded by [pseudonym] · [date] · [N] members · [JoinPolicy badge]
```

**Back link:** uses `commonsName` and `commonsId` from the detail response — no separate commons fetch required.

**Join button states:**

| Condition | UI |
|---|---|
| `joinPolicy = Open`, not member, authenticated | "Join" button |
| `joinPolicy = Approval`, not member, authenticated | "Request to Join" button |
| `joinPolicy = InviteOnly`, not member | No button; badge reads "Invite Only" |
| `isMember = true` | Inert "Member" badge; no button |
| Unauthenticated + Open or Approval | `AuthRequiredButton` wrapping the button |

---

### 2. Governance Charter (collapsible card, collapsed by default)

Collapsed by default. User expands to read.

Three sub-sections rendered as labelled key-value pairs:

**Membership Rules**
- Join policy
- Max members (omit if null)
- Entry requirements (omit if null)
- Member list: "Public" or "Private"
- Approval threshold + veto (omit if `approvalSettings` null)

**Contestation Rules**
- Resolution window
- Rejection threshold — rendered as percentage (e.g. `0.3` → "30%")

**Shunning Rules**
- Approval threshold — rendered as percentage

All fields read-only. Null/missing fields silently omitted.

---

### 3. Members (conditional)

Rendered only when `charter.membershipRules.memberListPublic = true`. Entirely absent otherwise — no "members are private" message.

**Initial load state:** skeleton list (3 placeholder rows) while the first page fetches.

- Heading: "Members (N)" — N from `memberCount` on the detail response, not the page count
- Each entry: clickable pseudonym → `/profiles/:pseudonym`
- Paginated via load-more button; page size: 20
- Load-more failure: inline error with retry, does not affect rest of page

---

### 4. Active Projects

```
Active Projects                         [View all projects →]

[Title]     [N active amendments]    [Contested]?
[description snippet, 2 lines max]
```

- Fetches via `GET /api/v1/usergroups/:id/projects?activeOnly=true`
- Each row links to `/projects/:projectId`
- "View all projects →" links to `/user-groups/:userGroupId/projects`
- Contested badge shown when `contestedAmendmentCount > 0`
- Load-more pattern; page size: 10
- **Empty state:** "No active projects." card (no action)
- Load-more failure: inline error with retry, does not affect rest of page

**Pagination note:** the existing `useProjectsForUserGroup` hook is a plain `useQuery` (no pagination). A new hook `useActiveProjectsForUserGroup` using `useInfiniteQuery` must be created for this page. The existing hook is left unchanged so `CommonsDetailPage` is unaffected.

---

## Component Structure

```
UserGroupDetailPage
├── useUserGroup(userGroupId)                 — fetches detail response
├── Header
│   └── JoinButton                            — encapsulates all join states
├── GovernanceCard (collapsible)
│   ├── MembershipRulesSection
│   ├── ContestationRulesSection
│   └── ShunningRulesSection
├── MembersSection (conditional on memberListPublic)
│   └── useUserGroupMembers(userGroupId)      — useInfiniteQuery, page size 20
└── ActiveProjectsSection
    └── useActiveProjectsForUserGroup(userGroupId)  — useInfiniteQuery, page size 10
```

Hooks in `src/BindingChaos.Web/src/shared/hooks/`.

---

## Error & Loading States

- Page-level loading: skeleton cards matching section heights (consistent with `ProjectDetailPage` pattern)
- 404 / no data: single card with message + back link to `/commons`
- Members initial load: skeleton list (3 rows)
- Members load-more failure: inline error with retry
- Projects load-more failure: inline error with retry

---

## Navigation Changes

### `CommonsDetailPage` > `UserGroupCard`

The group name becomes a `Link` to `/user-groups/:userGroupId`. The existing "View all (N)" projects link within the card is **retained** — it links directly to `/user-groups/:userGroupId/projects` as before. Both links coexist: the name goes to the group detail, "View all" goes to the full projects list.

### `ProfilePage` > `UserGroupsTab`

The `onClick` handler currently navigates to `/commons/${g.commonsId}`. Change to `/user-groups/${g.id}` (field is `id` on `UserGroupListItemResponse`, not `userGroupId`).

---

## Out of Scope

- Member management (kick, invite, approve join requests)
- Editing charter or group details
- Join request status tracking (pending approval)
- Discourse thread for the user group
