# Integration Tests Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add high-value integration tests to `BindingChaos.CorePlatform.API.IntegrationTests` for all untested controllers, covering auth guards, business logic, and Marten query behaviors deferred from unit tests.

**Architecture:** One test file per endpoint group inside `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/{Area}/`. All tests share a single `ApiFactory` via `[Collection("integration")]`. Setup data is created through the API in each test method.

**Tech Stack:** xUnit, FluentAssertions, Alba `WebApplicationFactory`, Testcontainers (PostgreSQL). Run with `npm run dotnet:test`.

**Stop and ask for guidance if:** the controller routes, request/response shapes, or command/query names do not match what is described in this plan. Do not guess or infer intent.

---

## Context 1: Stigmergy — UserGroups

### Task 1: FormUserGroup tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/FormUserGroupTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class FormUserGroupTests(ApiFactory factory)
{
    [Fact]
    public async Task FormUserGroup_without_token_returns_401()
    {
        var client = factory.CreateClient();
        var commonsId = await CreateCommonsAsync(factory.CreateClient().WithAuthToken());

        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            ValidFormUserGroupRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FormUserGroup_with_valid_request_returns_201_with_location()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            ValidFormUserGroupRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/usergroups/");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons for user group tests."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<BindingChaos.Infrastructure.API.ApiResponse<string>>(
            TestContext.Current.CancellationToken);
        return body!.Data!;
    }

    private static FormUserGroupRequest ValidFormUserGroupRequest(string commonsId) => new(
        CommonsId: commonsId,
        Name: "Test User Group",
        Philosophy: "We exist to govern this commons fairly.",
        Charter: new UserGroupCharterDto(
            ContestationRules: new UserGroupContestationRulesDto(
                ResolutionWindow: TimeSpan.FromDays(3),
                RejectionThreshold: 0.4m),
            MembershipRules: new UserGroupMembershipRulesDto(
                JoinPolicy: UserGroupJoinPolicyDto.Open,
                ApprovalSettings: null,
                MaxMembers: null,
                EntryRequirements: null,
                MemberListPublic: true),
            ShunningRules: new UserGroupShunningRulesDto(ApprovalThreshold: 0.6m)));
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/FormUserGroupTests.cs
git commit -m "test(integration): add FormUserGroup tests"
```

---

### Task 2: GetUserGroupsForCommons tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupsForCommonsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupsForCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetUserGroupsForCommons_without_commonsId_returns_400()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/usergroups",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserGroupsForCommons_returns_group_formed_for_commons()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        await FormUserGroupAsync(client, commonsId, "Retrievable Group");

        var response = await client.GetAsync(
            $"/api/usergroups?commonsId={commonsId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(g => g.Name == "Retrievable Group");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "Commons for list tests."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task FormUserGroupAsync(HttpClient client, string commonsId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: name,
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupsForCommonsTests.cs
git commit -m "test(integration): add GetUserGroupsForCommons tests"
```

---

### Task 3: GetMyUserGroups tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetMyUserGroupsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetMyUserGroupsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetMyUserGroups_without_token_returns_401()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/usergroups/mine",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyUserGroups_returns_only_groups_founded_by_caller()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var callerClient = factory.CreateClient().WithAuthToken(participantId);
        var otherClient = factory.CreateClient().WithAuthToken();

        // Caller forms a group
        var commonsId = await CreateCommonsAsync(callerClient);
        await FormUserGroupAsync(callerClient, commonsId, "Caller Group");

        // Another participant forms a different group
        var otherCommonsId = await CreateCommonsAsync(otherClient);
        await FormUserGroupAsync(otherClient, otherCommonsId, "Other Group");

        var response = await callerClient.GetAsync(
            "/api/usergroups/mine",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupListItemResponse[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(g => g.Name == "Caller Group");
        body.Data.Should().NotContain(g => g.Name == "Other Group");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task FormUserGroupAsync(HttpClient client, string commonsId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: name,
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetMyUserGroupsTests.cs
git commit -m "test(integration): add GetMyUserGroups tests"
```

---

### Task 4: GetUserGroupDetail tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupDetailTests.cs`

These tests cover the Marten `IQueryable` paths deferred from `GetUserGroupDetailHandlerTests`.

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupDetailTests(ApiFactory factory)
{
    private const string NonExistentId = "usergroup-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetUserGroupDetail_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/{NonExistentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserGroupDetail_returns_group_data()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        var groupId = await FormUserGroupAsync(client, commonsId, "Detail Test Group");

        var response = await client.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Name.Should().Be("Detail Test Group");
        body.Data.CommonsId.Should().Be(commonsId);
    }

    [Fact]
    public async Task GetUserGroupDetail_IsMember_is_true_for_founding_member()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken(participantId);
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, "Membership Check Group");

        var response = await founderClient.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.IsMember.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserGroupDetail_IsMember_is_false_for_non_member()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, "Non Member Group");

        // Different participant queries the group
        var outsiderClient = factory.CreateClient().WithAuthToken();
        var response = await outsiderClient.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.IsMember.Should().BeFalse();
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<string> FormUserGroupAsync(HttpClient client, string commonsId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: name,
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupDetailTests.cs
git commit -m "test(integration): add GetUserGroupDetail tests (deferred Marten query)"
```

---

### Task 5: GetUserGroupMembers tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupMembersTests.cs`

These tests cover the private member list access control and the Marten paginated query paths deferred from `GetUserGroupMembersHandlerTests`.

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupMembersTests(ApiFactory factory)
{
    private const string NonExistentId = "usergroup-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetUserGroupMembers_for_nonexistent_group_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/{NonExistentId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_public_group_returns_members()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        var groupId = await FormUserGroupAsync(client, commonsId, memberListPublic: true);

        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupMemberResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_private_group_and_non_member_returns_403()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, memberListPublic: false);

        var outsiderClient = factory.CreateClient().WithAuthToken();
        var response = await outsiderClient.GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_private_group_and_member_returns_members()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken(participantId);
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, memberListPublic: false);

        // Founder is a member — should see the list
        var response = await founderClient.GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupMemberResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<string> FormUserGroupAsync(HttpClient client, string commonsId, bool memberListPublic)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: "Members Test Group",
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, memberListPublic),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupMembersTests.cs
git commit -m "test(integration): add GetUserGroupMembers tests (deferred Marten query + access control)"
```

---

### Task 6: GetUserGroupsForParticipant tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupsForParticipantTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupsForParticipantTests(ApiFactory factory)
{
    [Fact]
    public async Task GetUserGroupsForParticipant_without_participantId_returns_400()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/usergroups/for-participant",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUserGroupsForParticipant_returns_groups_for_participant()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var client = factory.CreateClient().WithAuthToken(participantId);

        var commonsId = await CreateCommonsAsync(client);
        await FormUserGroupAsync(client, commonsId, "Participant Group");

        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/for-participant?participantId={participantId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupListItemResponse[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(g => g.Name == "Participant Group");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task FormUserGroupAsync(HttpClient client, string commonsId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: name,
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/UserGroups/GetUserGroupsForParticipantTests.cs
git commit -m "test(integration): add GetUserGroupsForParticipant tests"
```

---

## Context 2: Stigmergy — Projects

Projects require a UserGroup as setup, which requires a Commons. All project test helpers follow the same chain: Commons → UserGroup → Project.

### Task 7: CreateProject tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateProjectTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateProjectTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateProject_without_token_returns_401()
    {
        var setupClient = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateUserGroupAsync(setupClient);

        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Test Project", "A project description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProject_with_valid_request_returns_201_with_location()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateUserGroupAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Test Project", "A project description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/projects/");
    }

    internal static async Task<string> CreateUserGroupAsync(HttpClient client)
    {
        var commonsResponse = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Project Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        commonsResponse.EnsureSuccessStatusCode();
        var commonsId = (await commonsResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var groupResponse = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: "Project Test Group",
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        groupResponse.EnsureSuccessStatusCode();
        return (await groupResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateProjectTests.cs
git commit -m "test(integration): add CreateProject tests"
```

---

### Task 8: GetProject tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetProjectTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetProjectTests(ApiFactory factory)
{
    private const string NonExistentId = "project-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetProject_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{NonExistentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProject_returns_created_project()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);

        var createResponse = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Get Test Project", "Description."),
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var projectId = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Title.Should().Be("Get Test Project");
        body.Data.UserGroupId.Should().Be(userGroupId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetProjectTests.cs
git commit -m "test(integration): add GetProject tests"
```

---

### Task 9: GetProjects tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetProjectsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetProjectsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetProjects_without_userGroupId_returns_400()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/projects",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProjects_returns_projects_scoped_to_user_group()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);

        await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Group Scoped Project", "Description."),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects?userGroupId={userGroupId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProjectListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(p => p.Title == "Group Scoped Project");
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetProjectsTests.cs
git commit -m "test(integration): add GetProjects tests"
```

---

### Task 10: CreateAmendment tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateAmendmentTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateAmendmentTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAmendment_creates_amendment_and_returns_201()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var amendmentId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        amendmentId.Should().NotBeNullOrWhiteSpace();
    }

    private static async Task<string> CreateProjectAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Amendment Test Project", "Description."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateAmendmentTests.cs
git commit -m "test(integration): add CreateAmendment tests"
```

---

### Task 11: ContestAmendment tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/ContestAmendmentTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class ContestAmendmentTests(ApiFactory factory)
{
    [Fact]
    public async Task ContestAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, amendmentId) = await CreateProjectWithAmendmentAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments/{amendmentId}/contests",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ContestAmendment_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, amendmentId) = await CreateProjectWithAmendmentAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments/{amendmentId}/contests",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<(string projectId, string amendmentId)> CreateProjectWithAmendmentAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var projectResponse = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Contest Test Project", "Description."),
            TestContext.Current.CancellationToken);
        projectResponse.EnsureSuccessStatusCode();
        var projectId = (await projectResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var amendmentResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);
        amendmentResponse.EnsureSuccessStatusCode();
        var amendmentId = (await amendmentResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        return (projectId, amendmentId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/ContestAmendmentTests.cs
git commit -m "test(integration): add ContestAmendment tests"
```

---

### Task 12: CreateInquiry tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateInquiryTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateInquiryTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateInquiry_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Why is this project needed?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateInquiry_returns_201_with_inquiry_id()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Why is this project needed?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var inquiryId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        inquiryId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateProjectAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Inquiry Test Project", "Description."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/CreateInquiryTests.cs
git commit -m "test(integration): add CreateInquiry tests"
```

---

### Task 13: GetInquiries tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetInquiriesTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetInquiriesTests(ApiFactory factory)
{
    [Fact]
    public async Task GetInquiries_returns_raised_inquiry()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("What is the scope of this project?"),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}/inquiries",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProjectInquiryResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(i => i.Body == "What is the scope of this project?");
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetInquiriesTests.cs
git commit -m "test(integration): add GetInquiries tests"
```

---

### Task 14: GetInquiry tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetInquiryTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetInquiryTests(ApiFactory factory)
{
    private const string NonExistentInquiryId = "inquiry-01aryz6s41tptwgy5wdss56yxy";
    private const string SomProjectId = "project-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetInquiry_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{SomProjectId}/inquiries/{NonExistentInquiryId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetInquiry_returns_raised_inquiry()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        var createResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Specific inquiry body."),
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var inquiryId = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectInquiryResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Body.Should().Be("Specific inquiry body.");
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/GetInquiryTests.cs
git commit -m "test(integration): add GetInquiry tests"
```

---

### Task 15: PostInquiryResponse tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/PostInquiryResponseTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class PostInquiryResponseTests(ApiFactory factory)
{
    [Fact]
    public async Task PostInquiryResponse_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithInquiryAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/responses",
            new RespondToProjectInquiryRequest("Here is the group's response."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostInquiryResponse_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithInquiryAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/responses",
            new RespondToProjectInquiryRequest("Here is the group's response."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<(string projectId, string inquiryId)> CreateProjectWithInquiryAsync(HttpClient client)
    {
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        var inquiryResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("What is the timeline?"),
            TestContext.Current.CancellationToken);
        inquiryResponse.EnsureSuccessStatusCode();
        var inquiryId = (await inquiryResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
        return (projectId, inquiryId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/PostInquiryResponseTests.cs
git commit -m "test(integration): add PostInquiryResponse tests"
```

---

### Task 16: PostInquiryResolution tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/PostInquiryResolutionTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class PostInquiryResolutionTests(ApiFactory factory)
{
    [Fact]
    public async Task PostInquiryResolution_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithRespondedInquiryAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/resolutions",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostInquiryResolution_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithRespondedInquiryAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/resolutions",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<(string projectId, string inquiryId)> CreateProjectWithRespondedInquiryAsync(HttpClient client)
    {
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        var inquiryResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Is there a budget?"),
            TestContext.Current.CancellationToken);
        inquiryResponse.EnsureSuccessStatusCode();
        var inquiryId = (await inquiryResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        // Respond before resolving
        var respondResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/responses",
            new RespondToProjectInquiryRequest("Yes, there is a budget."),
            TestContext.Current.CancellationToken);
        respondResponse.EnsureSuccessStatusCode();

        return (projectId, inquiryId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Projects/PostInquiryResolutionTests.cs
git commit -m "test(integration): add PostInquiryResolution tests"
```

---

## Context 3: Stigmergy — Commons & Concerns

### Task 17: ProposeCommons tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/ProposeCommonsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class ProposeCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task ProposeCommons_without_token_returns_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Some Commons", "A description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProposeCommons_returns_201_with_commons_id()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("New Commons", "A description of the commons."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var commonsId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        commonsId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/ProposeCommonsTests.cs
git commit -m "test(integration): add ProposeCommons tests"
```

---

### Task 18: GetCommons tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/GetCommonsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class GetCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetCommons_returns_proposed_commons()
    {
        var client = factory.CreateClient().WithAuthToken();
        await ProposeCommonsTests.CreateCommonsAsync(client);

        var response = await factory.CreateClient().GetAsync(
            "/api/commons",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<CommonsListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/GetCommonsTests.cs
git commit -m "test(integration): add GetCommons tests"
```

---

### Task 19: LinkConcernToCommons tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/LinkConcernToCommonsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class LinkConcernToCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task LinkConcernToCommons_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await ProposeCommonsTests.CreateCommonsAsync(client);
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/commons/{commonsId}/concerns/{concernId}",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LinkConcernToCommons_returns_204_and_concern_appears_in_list()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await ProposeCommonsTests.CreateCommonsAsync(client);
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var linkResponse = await client.PostAsJsonAsync(
            $"/api/commons/{commonsId}/concerns/{concernId}",
            new { },
            TestContext.Current.CancellationToken);
        linkResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var listResponse = await client.GetAsync(
            $"/api/commons/{commonsId}/concerns",
            TestContext.Current.CancellationToken);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await listResponse.Content.ReadFromJsonAsync<ApiResponse<List<ConcernListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(c => c.Id == concernId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/LinkConcernToCommonsTests.cs
git commit -m "test(integration): add LinkConcernToCommons tests"
```

---

### Task 20: GetConcernsForCommons tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/GetConcernsForCommonsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class GetConcernsForCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetConcernsForCommons_returns_linked_concerns()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await ProposeCommonsTests.CreateCommonsAsync(client);
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        await client.PostAsJsonAsync(
            $"/api/commons/{commonsId}/concerns/{concernId}",
            new { },
            TestContext.Current.CancellationToken);

        var response = await client.GetAsync(
            $"/api/commons/{commonsId}/concerns",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ConcernListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(c => c.Id == concernId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Commons/GetConcernsForCommonsTests.cs
git commit -m "test(integration): add GetConcernsForCommons tests"
```

---

### Task 21: CreateConcern tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/CreateConcernTests.cs`

Note: `RaiseConcern` does not check for anonymous — any caller can raise a concern. There is no 401 guard to test on this endpoint.

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;

[Collection("integration")]
public class CreateConcernTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateConcern_returns_201_with_concern_id()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/concerns",
            ValidRaiseConcernRequest(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var concernId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        concernId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateConcernAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/concerns",
            ValidRaiseConcernRequest(),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static RaiseConcernRequest ValidRaiseConcernRequest() => new(
        Name: "Test Concern",
        Tags: ["environment"],
        SignalIds: [],
        Origin: ConcernOriginDto.Manual);
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/CreateConcernTests.cs
git commit -m "test(integration): add CreateConcern tests"
```

---

### Task 22: GetConcerns tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/GetConcernsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;

[Collection("integration")]
public class GetConcernsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetConcerns_returns_raised_concern()
    {
        var client = factory.CreateClient().WithAuthToken();
        await CreateConcernTests.CreateConcernAsync(client);

        var response = await factory.CreateClient().GetAsync(
            "/api/concerns",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ConcernListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/GetConcernsTests.cs
git commit -m "test(integration): add GetConcerns tests"
```

---

### Task 23: AddAffectedness tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/AddAffectednessTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;

[Collection("integration")]
public class AddAffectednessTests(ApiFactory factory)
{
    [Fact]
    public async Task AddAffectedness_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/concerns/{concernId}/affectedness",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddAffectedness_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/concerns/{concernId}/affectedness",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/AddAffectednessTests.cs
git commit -m "test(integration): add AddAffectedness tests"
```

---

### Task 24: RemoveAffectedness tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/RemoveAffectednessTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;

[Collection("integration")]
public class RemoveAffectednessTests(ApiFactory factory)
{
    [Fact]
    public async Task RemoveAffectedness_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var response = await factory.CreateClient().DeleteAsync(
            $"/api/concerns/{concernId}/affectedness",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveAffectedness_after_declaring_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        // Declare first
        await client.PostAsJsonAsync(
            $"/api/concerns/{concernId}/affectedness",
            new { },
            TestContext.Current.CancellationToken);

        var response = await client.DeleteAsync(
            $"/api/concerns/{concernId}/affectedness",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Concerns/RemoveAffectednessTests.cs
git commit -m "test(integration): add RemoveAffectedness tests"
```

---

## Context 4: Societies

For `JoinSociety`, the request requires a `SocialContractId`. The `GET /api/societies/{id}` response includes `CurrentSocialContractId`. Retrieve it after creating the society to get the ID needed for the join request.

### Task 25: CreateSociety tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/CreateSocietyTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class CreateSocietyTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateSociety_without_token_returns_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/societies",
            ValidCreateSocietyRequest(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSociety_returns_201_with_location()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/societies",
            ValidCreateSocietyRequest(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    internal static async Task<(string societyId, string socialContractId)> CreateSocietyAsync(HttpClient client)
    {
        var createResponse = await client.PostAsJsonAsync(
            "/api/societies",
            ValidCreateSocietyRequest(),
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var societyId = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        // Retrieve the social contract ID needed for joining
        var detailResponse = await client.GetAsync(
            $"/api/societies/{societyId}",
            TestContext.Current.CancellationToken);
        detailResponse.EnsureSuccessStatusCode();
        var detail = (await detailResponse.Content.ReadFromJsonAsync<ApiResponse<SocietyResponse>>(TestContext.Current.CancellationToken))!.Data!;

        return (societyId, detail.CurrentSocialContractId!);
    }

    private static CreateSocietyRequest ValidCreateSocietyRequest() => new(
        Name: "Test Society",
        Description: "A society for integration tests.",
        Tags: ["test"],
        RatificationThreshold: 0.5,
        ReviewWindowHours: 48,
        AllowVeto: false,
        RequiredVerificationWeight: 0,
        InquiryLapseWindowHours: 72,
        GeographicBounds: null,
        Center: null);
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/CreateSocietyTests.cs
git commit -m "test(integration): add CreateSociety tests"
```

---

### Task 26: GetSocieties tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietiesTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class GetSocietiesTests(ApiFactory factory)
{
    [Fact]
    public async Task GetSocieties_returns_created_society()
    {
        var client = factory.CreateClient().WithAuthToken();
        await CreateSocietyTests.CreateSocietyAsync(client);

        var response = await factory.CreateClient().GetAsync(
            "/api/societies",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<SocietyListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietiesTests.cs
git commit -m "test(integration): add GetSocieties tests"
```

---

### Task 27: GetSociety tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietyTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class GetSocietyTests(ApiFactory factory)
{
    private const string NonExistentId = "society-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetSociety_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/societies/{NonExistentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSociety_returns_society_details()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(client);

        var response = await factory.CreateClient().GetAsync(
            $"/api/societies/{societyId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SocietyResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Id.Should().Be(societyId);
        body.Data.Name.Should().Be("Test Society");
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietyTests.cs
git commit -m "test(integration): add GetSociety tests"
```

---

### Task 28: JoinSociety tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/JoinSocietyTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class JoinSocietyTests(ApiFactory factory)
{
    [Fact]
    public async Task JoinSociety_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task JoinSociety_with_valid_contract_returns_201()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(founderClient);

        var joinerClient = factory.CreateClient().WithAuthToken();
        var response = await joinerClient.PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/JoinSocietyTests.cs
git commit -m "test(integration): add JoinSociety tests"
```

---

### Task 29: GetSocietyMembers tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietyMembersTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class GetSocietyMembersTests(ApiFactory factory)
{
    [Fact]
    public async Task GetSocietyMembers_returns_member_after_join()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(founderClient);

        var joinerClient = factory.CreateClient().WithAuthToken();
        await joinerClient.PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().GetAsync(
            $"/api/societies/{societyId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<SocietyMemberResponse>>>(
            TestContext.Current.CancellationToken);
        // Founder + joiner = at least 2 members
        body!.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetSocietyMembersTests.cs
git commit -m "test(integration): add GetSocietyMembers tests"
```

---

### Task 30: GetMySocietyIds tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetMySocietyIdsTests.cs`

Note: this endpoint is `[AllowAnonymous]` and returns an empty array for unauthenticated callers, not 401.

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class GetMySocietyIdsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetMySocietyIds_unauthenticated_returns_empty_array()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/societies/memberships/me",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<string[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMySocietyIds_returns_ids_of_joined_societies()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var client = factory.CreateClient().WithAuthToken(participantId);
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(client);

        await client.PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        var response = await client.GetAsync(
            "/api/societies/memberships/me",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<string[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(societyId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/GetMySocietyIdsTests.cs
git commit -m "test(integration): add GetMySocietyIds tests"
```

---

### Task 31: LeaveSociety tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/LeaveSocietyTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class LeaveSocietyTests(ApiFactory factory)
{
    [Fact]
    public async Task LeaveSociety_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(client);

        var joiner = factory.CreateClient().WithAuthToken();
        await joiner.PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().DeleteAsync(
            $"/api/societies/{societyId}/memberships/me",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LeaveSociety_removes_membership_from_my_list()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken();
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(founderClient);

        var joinerClient = factory.CreateClient().WithAuthToken(participantId);
        await joinerClient.PostAsJsonAsync(
            $"/api/societies/{societyId}/memberships",
            new JoinSocietyRequest(contractId),
            TestContext.Current.CancellationToken);

        var leaveResponse = await joinerClient.DeleteAsync(
            $"/api/societies/{societyId}/memberships/me",
            TestContext.Current.CancellationToken);
        leaveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var membershipsResponse = await joinerClient.GetAsync(
            "/api/societies/memberships/me",
            TestContext.Current.CancellationToken);
        var body = await membershipsResponse.Content.ReadFromJsonAsync<ApiResponse<string[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().NotContain(societyId);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/LeaveSocietyTests.cs
git commit -m "test(integration): add LeaveSociety tests"
```

---

### Task 32: LinkCommonsToSociety tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/LinkCommonsToSocietyTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class LinkCommonsToSocietyTests(ApiFactory factory)
{
    [Fact]
    public async Task LinkCommonsToSociety_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(client);
        var commonsId = await BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons.ProposeCommonsTests.CreateCommonsAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/societies/{societyId}/commons-links",
            new DeclareSocietyAffectedByCommonsRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LinkCommonsToSociety_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(client);
        var commonsId = await BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons.ProposeCommonsTests.CreateCommonsAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/societies/{societyId}/commons-links",
            new DeclareSocietyAffectedByCommonsRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/LinkCommonsToSocietyTests.cs
git commit -m "test(integration): add LinkCommonsToSociety tests"
```

---

### Task 33: SocietyInviteLink tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/SocietyInviteLinkTests.cs`

Note: both create and get invite links require the caller to be an active member of the society (our membership check, not just auth).

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Societies;

[Collection("integration")]
public class SocietyInviteLinkTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateInviteLink_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/societies/{societyId}/invite-links",
            new CreateSocietyInviteLinkRequest(null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateInviteLink_by_non_member_returns_403()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(founderClient);

        var outsider = factory.CreateClient().WithAuthToken();
        var response = await outsider.PostAsJsonAsync(
            $"/api/societies/{societyId}/invite-links",
            new CreateSocietyInviteLinkRequest(null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateInviteLink_by_member_returns_201_and_link_appears_in_mine()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken(participantId);
        var (societyId, contractId) = await CreateSocietyTests.CreateSocietyAsync(founderClient);

        // Founder is already a member
        var createResponse = await founderClient.PostAsJsonAsync(
            $"/api/societies/{societyId}/invite-links",
            new CreateSocietyInviteLinkRequest("A note"),
            TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await founderClient.GetAsync(
            $"/api/societies/{societyId}/invite-links/mine",
            TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<List<SocietyInviteLinkViewResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMyInviteLinks_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (societyId, _) = await CreateSocietyTests.CreateSocietyAsync(client);

        var response = await factory.CreateClient().GetAsync(
            $"/api/societies/{societyId}/invite-links/mine",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Societies/SocietyInviteLinkTests.cs
git commit -m "test(integration): add SocietyInviteLink tests"
```

---

## Context 5: IdentityProfile — remaining endpoints

### Task 34: TrustInviteLink tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/IdentityProfiles/TrustInviteLinkTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.IdentityProfiles;

[Collection("integration")]
public class TrustInviteLinkTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateInviteLink_without_token_returns_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/identity/invite-links",
            new CreateTrustInviteLinkRequest { Note = "test" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateInviteLink_returns_201()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/identity/invite-links",
            new CreateTrustInviteLinkRequest { Note = "test note" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetMyInviteLinks_without_token_returns_401()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/identity/invite-links",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyInviteLinks_returns_created_link()
    {
        var client = factory.CreateClient().WithAuthToken();
        await client.PostAsJsonAsync(
            "/api/identity/invite-links",
            new CreateTrustInviteLinkRequest { Note = "listed note" },
            TestContext.Current.CancellationToken);

        var response = await client.GetAsync(
            "/api/identity/invite-links",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<TrustInviteLinkViewResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(l => l.Note == "listed note");
    }

    [Fact]
    public async Task ResolveInviteLink_with_valid_token_returns_inviter_id()
    {
        var client = factory.CreateClient().WithAuthToken();
        await client.PostAsJsonAsync(
            "/api/identity/invite-links",
            new CreateTrustInviteLinkRequest { Note = "resolve test" },
            TestContext.Current.CancellationToken);

        var links = await client.GetAsync("/api/identity/invite-links", TestContext.Current.CancellationToken);
        var body = await links.Content.ReadFromJsonAsync<ApiResponse<List<TrustInviteLinkViewResponse>>>(TestContext.Current.CancellationToken);
        var token = body!.Data!.First(l => l.Note == "resolve test").Token;

        var resolveResponse = await factory.CreateClient().GetAsync(
            $"/api/identity/invite-links/resolve?token={token}",
            TestContext.Current.CancellationToken);

        resolveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var resolveBody = await resolveResponse.Content.ReadFromJsonAsync<ApiResponse<ResolvedInviteLinkResponse>>(
            TestContext.Current.CancellationToken);
        resolveBody!.Data!.InviterUserId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ResolveInviteLink_with_invalid_token_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/identity/invite-links/resolve?token=invalid-token-xyz",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteInviteLink_without_token_returns_401()
    {
        var response = await factory.CreateClient().DeleteAsync(
            $"/api/identity/invite-links/{Guid.NewGuid()}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteInviteLink_removes_it_from_list()
    {
        var client = factory.CreateClient().WithAuthToken();
        var createResponse = await client.PostAsJsonAsync(
            "/api/identity/invite-links",
            new CreateTrustInviteLinkRequest { Note = "to delete" },
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();

        var links = await client.GetAsync("/api/identity/invite-links", TestContext.Current.CancellationToken);
        var body = await links.Content.ReadFromJsonAsync<ApiResponse<List<TrustInviteLinkViewResponse>>>(TestContext.Current.CancellationToken);
        var linkId = body!.Data!.First(l => l.Note == "to delete").Id;

        var deleteResponse = await client.DeleteAsync(
            $"/api/identity/invite-links/{linkId}",
            TestContext.Current.CancellationToken);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/IdentityProfiles/TrustInviteLinkTests.cs
git commit -m "test(integration): add TrustInviteLink tests"
```

---

### Task 35: Profiles tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Profiles/ProfileTests.cs`

Note: `GetProfile` and `GetProfileByUserId` require a participant to have been linked via `POST /api/identity/users/link`. Check `LinkTests.cs` for the existing pattern.

- [ ] **Step 1: Read existing link tests to understand the setup pattern**

Read `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/IdentityProfiles/LinkTests.cs` to understand how to create a linked participant with a known pseudonym before writing this test.

- [ ] **Step 2: Write the test file**

```csharp
using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Profiles;

[Collection("integration")]
public class ProfileTests(ApiFactory factory)
{
    [Fact]
    public async Task GetProfileByPseudonym_for_unknown_pseudonym_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/profiles/unknown-pseudonym-xyz-notexist",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProfileByUserId_for_unknown_user_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/profiles/by-user/unknown-user-id-xyz",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProfileByUserId_returns_profile_for_linked_user()
    {
        // Link a user to get a userId
        var linkResponse = await factory.CreateClient().PostAsJsonAsync(
            "/api/identity/users/link",
            new { Provider = "test", Subject = $"profile-test-subject-{Guid.NewGuid()}" },
            TestContext.Current.CancellationToken);
        linkResponse.EnsureSuccessStatusCode();
        var linkBody = await linkResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        // The response is { "data": { "userId": "..." } }
        using var doc = System.Text.Json.JsonDocument.Parse(linkBody);
        var userId = doc.RootElement.GetProperty("data").GetProperty("userId").GetString()!;

        var response = await factory.CreateClient().GetAsync(
            $"/api/profiles/by-user/{userId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ParticipantProfileResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.UserId.Should().Be(userId);
    }
}
```

- [ ] **Step 3: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 4: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/Profiles/ProfileTests.cs
git commit -m "test(integration): add Profile endpoint tests"
```

---

## Context 6: Stigmergy — EmergingPatterns

### Task 36: GetEmergingPatterns tests

**Files:**
- Create: `src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/EmergingPatterns/GetEmergingPatternsTests.cs`

- [ ] **Step 1: Write the test file**

```csharp
using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.EmergingPatterns;

[Collection("integration")]
public class GetEmergingPatternsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetEmergingPatterns_returns_200_with_patterns_shape()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/emerging-patterns",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using var doc = System.Text.Json.JsonDocument.Parse(body);
        doc.RootElement.GetProperty("data").GetProperty("patterns").ValueKind
            .Should().Be(System.Text.Json.JsonValueKind.Array);
    }
}
```

- [ ] **Step 2: Run tests**

```
npm run dotnet:test
```

Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/BindingChaos.CorePlatform.API.IntegrationTests/Endpoints/EmergingPatterns/GetEmergingPatternsTests.cs
git commit -m "test(integration): add GetEmergingPatterns tests"
```
