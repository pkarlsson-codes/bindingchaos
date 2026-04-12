namespace BindingChaos.Stigmergy.Tests.Application.Queries;

// NOTE: GetUserGroupDetailHandler uses IQuerySession.Query<T>() (UserGroupListItemView,
// CommonsListItemView) with async LINQ operators against IMartenQueryable<T>.
//
// Marten's IQuerySession.Query<T>() returns IMartenQueryable<T>, which cannot be backed by a
// plain in-memory IQueryable<T> in Moq without implementing the full Marten query provider.
//
// Handler-level tests are deferred to BindingChaos.CorePlatform.API.IntegrationTests,
// which exercises the full Marten stack against a real PostgreSQL instance.
//
// No tests are declared in this file intentionally.
public class GetUserGroupDetailHandlerTests;
