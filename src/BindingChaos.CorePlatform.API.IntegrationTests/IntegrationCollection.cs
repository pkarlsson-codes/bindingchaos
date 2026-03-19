namespace BindingChaos.CorePlatform.API.IntegrationTests;

/// <summary>
/// Defines the xUnit collection that shares a single <see cref="ApiFactory"/> instance
/// across all integration test classes.
///
/// All test classes that use [Collection("integration")] will receive the same
/// factory — meaning the ASP.NET Core test host is started once and reused,
/// which avoids parallel startup races against the test database and keeps
/// the test suite fast.
/// </summary>
[CollectionDefinition("integration")]
public class IntegrationCollection : ICollectionFixture<ApiFactory>;
