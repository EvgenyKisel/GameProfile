using Xunit;

namespace GameProfile.Tests.Fixtures;

[CollectionDefinition(nameof(ApiTestCollection))]
public class ApiTestCollection : ICollectionFixture<LoginFixture>;
