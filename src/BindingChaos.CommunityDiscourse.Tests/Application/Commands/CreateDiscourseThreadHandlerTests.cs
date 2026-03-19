// NOTE: CreateDiscourseThreadHandler uses IQuerySession.Query<DiscourseThreadView>() and chains
// Marten-specific LINQ operators (.Where / .OrderByDescending / .ThenByDescending) followed by
// Marten's own FirstOrDefaultAsync extension method on IMartenQueryable<T>.
//
// Marten's FirstOrDefaultAsync is NOT the standard EF/LINQ async extension — it is implemented
// internally by Marten's query provider and cannot be set up via Moq without standing up a real
// Marten document session.  Attempting to back the mock with an in-memory IQueryProvider causes
// FirstOrDefaultAsync to throw at runtime even though the code compiles.
//
// For this reason all handler-level tests for CreateDiscourseThreadHandler are deferred to the
// BindingChaos.CorePlatform.API.IntegrationTests project, which exercises the full Marten stack
// against a real PostgreSQL instance.  Unit-testing this particular handler would require either:
//   a) A wrapper/abstraction around the Marten query (e.g. IDiscourseThreadReadRepository), or
//   b) A real Marten test harness (integration test).
//
// No tests are declared in this file intentionally.
