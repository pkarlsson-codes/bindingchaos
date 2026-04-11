// NOTE: RaiseProjectInquiryHandler uses IQuerySession.Query<T>() to perform cross-BC standing
// checks (UserGroupListItemView, SocietyAffectedByCommonsView, SocietyMemberView,
// SocialContractView) using synchronous LINQ operators against IMartenQueryable<T>.
//
// Marten's IQuerySession.Query<T>() returns IMartenQueryable<T>, which cannot be backed by a
// plain in-memory IQueryable<T> in Moq without implementing the full Marten query provider.
//
// For this reason, handler-level tests for RaiseProjectInquiryHandler are deferred to the
// BindingChaos.CorePlatform.API.IntegrationTests project, which exercises the full Marten stack
// against a real PostgreSQL instance.
//
// No tests are declared in this file intentionally.
