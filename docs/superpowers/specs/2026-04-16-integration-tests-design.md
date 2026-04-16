# Integration Tests Design

**Date:** 2026-04-16

## Goal

Add high-value integration tests to `BindingChaos.CorePlatform.API.IntegrationTests` for all controllers not yet covered. High-value means tests that exercise our own business logic, auth guards, and behaviors that cannot be unit-tested in isolation (e.g. Marten `IQueryable` queries deferred from unit test projects). Tests that only exercise FluentValidation, framework routing, or empty-result paths are excluded.

## Existing Coverage

The following areas already have adequate integration test coverage and are out of scope:

- **Signals** — Capture, Amplify, Deamplify, AddEvidence, GetSignals
- **Ideas** — AuthorIdea, GetIdea, GetIdeas
- **Discourse** — PostContribution, GetContributions, GetContributionsByEntity, GetContributionReplies
- **Identity** — GetUser, Link
- **Tagging** — GetTags

## Skip Criteria

The following are not high-value and should be omitted:

- 422 responses triggered purely by FluentValidation (not our own guards)
- Empty-list responses with no interesting logic
- Pure framework routing/serialization behavior

## Task Granularity

One plan task per endpoint group (one test file per endpoint). This matches the project's existing convention and makes tasks independently reviewable.

## Agent Guidance

If code encountered during implementation does not align with this spec (missing routes, renamed commands, different contracts, etc.), **stop and ask for guidance** before proceeding. Do not infer or guess intent.

---

## Contexts & Tests

### 1. Stigmergy — UserGroups

**Order: first** — most recently added; `GetUserGroupDetail` and paginated `GetUserGroupMembers` paths explicitly deferred from unit tests due to Marten `IQueryable`.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/UserGroups/FormUserGroupTests.cs` | `POST /api/usergroups` | Happy path → 201 + Location header with group ID; 401 when unauthenticated |
| `Endpoints/UserGroups/GetUserGroupsForCommonsTests.cs` | `GET /api/usergroups?commonsId=` | Returns formed group in paginated list; 400 when `commonsId` query param missing |
| `Endpoints/UserGroups/GetMyUserGroupsTests.cs` | `GET /api/usergroups/mine` | Returns only the caller's groups, not unrelated groups; 401 when unauthenticated |
| `Endpoints/UserGroups/GetUserGroupDetailTests.cs` | `GET /api/usergroups/{id}` | Happy path (full Marten query round-trip — deferred); 404 for nonexistent ID; `IsMember=true` when caller is a member of the group |
| `Endpoints/UserGroups/GetUserGroupMembersTests.cs` | `GET /api/usergroups/{id}/members` | Public member list → returns results; private list + non-member caller → 403; private list + authenticated member → returns results (deferred Marten query); nonexistent group → 404 |
| `Endpoints/UserGroups/GetUserGroupsForParticipantTests.cs` | `GET /api/usergroups/for-participant` | Happy path; 400 when `participantId` query param missing |

### 2. Stigmergy — Projects

**Order: second** — complex domain with inquiry/amendment flows; requires user group as setup prerequisite.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/Projects/CreateProjectTests.cs` | `POST /api/projects` | Happy path → 201 + Location header; 401 when unauthenticated |
| `Endpoints/Projects/GetProjectTests.cs` | `GET /api/projects/{id}` | Happy path; 404 for nonexistent ID |
| `Endpoints/Projects/GetProjectsTests.cs` | `GET /api/projects` | Returns projects scoped to user group; project status filter applied correctly |
| `Endpoints/Projects/CreateAmendmentTests.cs` | `POST /api/projects/{id}/amendments` | Happy path — amendment created and reflected on project |
| `Endpoints/Projects/ContestAmendmentTests.cs` | `POST /api/projects/{id}/amendments/{amendmentId}/contests` | Happy path — amendment contested |
| `Endpoints/Projects/CreateInquiryTests.cs` | `POST /api/projects/{id}/inquiries` | Happy path |
| `Endpoints/Projects/GetInquiriesTests.cs` | `GET /api/projects/{id}/inquiries` | Returns inquiry list for the project |
| `Endpoints/Projects/GetInquiryTests.cs` | `GET /api/projects/{id}/inquiries/{inquiryId}` | Happy path; 404 for nonexistent ID |
| `Endpoints/Projects/PostInquiryResponseTests.cs` | `POST /api/projects/{id}/inquiries/{inquiryId}/responses` | Happy path |
| `Endpoints/Projects/PostInquiryResolutionTests.cs` | `POST /api/projects/{id}/inquiries/{inquiryId}/resolutions` | Happy path |

### 3. Stigmergy — Commons & Concerns

**Order: third** — prerequisite for Societies (commons-link) so tested before Societies.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/Commons/ProposeCommonsTests.cs` | `POST /api/commons` | Happy path; 401 when unauthenticated |
| `Endpoints/Commons/GetCommonsTests.cs` | `GET /api/commons` | Returns proposed commons |
| `Endpoints/Commons/LinkConcernToCommonsTests.cs` | `POST /api/commons/{commonsId}/concerns/{concernId}` | Happy path — concern linked and returned in subsequent query |
| `Endpoints/Commons/GetConcernsForCommonsTests.cs` | `GET /api/commons/{commonsId}/concerns` | Returns concerns linked to the commons |
| `Endpoints/Concerns/CreateConcernTests.cs` | `POST /api/concerns` | Happy path; 401 when unauthenticated |
| `Endpoints/Concerns/GetConcernsTests.cs` | `GET /api/concerns` | Returns concerns list |
| `Endpoints/Concerns/AddAffectednessTests.cs` | `POST /api/concerns/{concernId}/affectedness` | Happy path |
| `Endpoints/Concerns/RemoveAffectednessTests.cs` | `DELETE /api/concerns/{concernId}/affectedness` | Happy path — affectedness removed |

### 4. Societies

**Order: fourth** — depends on Commons existing for commons-link tests.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/Societies/CreateSocietyTests.cs` | `POST /api/societies` | Happy path; 401 when unauthenticated |
| `Endpoints/Societies/GetSocietiesTests.cs` | `GET /api/societies` | Returns created society in list |
| `Endpoints/Societies/GetSocietyTests.cs` | `GET /api/societies/{id}` | Happy path; 404 for nonexistent ID |
| `Endpoints/Societies/GetSocietyMembersTests.cs` | `GET /api/societies/{id}/members` | Returns members after a participant has joined |
| `Endpoints/Societies/JoinSocietyTests.cs` | `POST /api/societies/{id}/memberships` | Join open society; join via invite link |
| `Endpoints/Societies/GetMyMembershipsTests.cs` | `GET /api/societies/memberships/me` | Returns only caller's memberships, not others'; 401 when unauthenticated |
| `Endpoints/Societies/LeaveSocietyTests.cs` | `DELETE /api/societies/{id}/memberships/me` | Leave → society no longer returned in caller's memberships |
| `Endpoints/Societies/LinkCommonsToSocietyTests.cs` | `POST /api/societies/{id}/commons-links` | Happy path — commons linked to society |
| `Endpoints/Societies/GetMyInviteLinksTests.cs` | `GET /api/societies/{id}/invite-links/mine` | Returns the link created by the caller |
| `Endpoints/Societies/CreateSocietyInviteLinkTests.cs` | `POST /api/societies/{id}/invite-links` | Creates link; 401 when unauthenticated |

### 5. IdentityProfile — remaining endpoints

**Order: fifth** — invite link resolution needed for Societies join-via-link, but that test can use the already-tested `POST /identity/invite-links`.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/IdentityProfiles/ResolveInviteLinkTests.cs` | `GET /identity/invite-links/resolve` | Resolves valid code → returns link data; 404 for invalid code |
| `Endpoints/IdentityProfiles/GetInviteLinksTests.cs` | `GET /identity/invite-links` | Returns links created by the caller; 401 when unauthenticated |
| `Endpoints/IdentityProfiles/CreateInviteLinkTests.cs` | `POST /identity/invite-links` | Creates link; 401 when unauthenticated |
| `Endpoints/IdentityProfiles/DeleteInviteLinkTests.cs` | `DELETE /identity/invite-links/{id}` | Deletes link; 404 for nonexistent ID |
| `Endpoints/Profiles/GetProfileByPseudonymTests.cs` | `GET /api/profiles/{pseudonym}` | Happy path; 404 for unknown pseudonym |
| `Endpoints/Profiles/GetProfileByUserTests.cs` | `GET /api/profiles/by-user/{userId}` | Happy path |

### 6. Stigmergy — EmergingPatterns

**Order: last** — low complexity; single smoke test verifying wiring.

| File | Endpoint | High-value scenarios |
|------|----------|----------------------|
| `Endpoints/EmergingPatterns/GetEmergingPatternsTests.cs` | `GET /api/emerging-patterns` | Endpoint wires to handler and returns a valid response shape |
