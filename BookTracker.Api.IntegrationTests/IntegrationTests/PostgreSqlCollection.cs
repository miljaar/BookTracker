namespace BookTracker.Api.IntegrationTests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "PostgreSQL integration tests";
}