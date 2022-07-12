using System.Threading.Tasks;
using EzDomain.EventSourcing.EventStores.Sql.Tests.IntegrationTests;
using EzDomain.EventSourcing.EventStores.SqlServer.Factories;
using NUnit.Framework;

namespace EzDomain.EventSourcing.EventStores.SqlServer.Tests.IntegrationTests;

[TestFixture("Server=.\\SQLEXPRESS; Trusted_Connection=True;", "Server=.\\SQLEXPRESS; Database=EventStoreTests; Trusted_Connection=True;")]
public sealed class SqlServerEventStoreTests
    : SqlEventStoreTests<SqlServerConnectionFactory>
{
    public SqlServerEventStoreTests(string initialConnectionString, string connectionString)
        : base(initialConnectionString, connectionString)
    {
    }

    [Test]
    public async Task GIVEN_events_to_save_WHEN_executing_calling_store_methods_THEN_initializes_event_store_AND_saves_events_AND_gets_events_from_event_store()
    {
        var eventStore = new SqlServerEventStore<string>(Settings, SqlStatementsLoader, new SqlServerConnectionFactory(), MockEventDataSerializer.Object);

        await ExecuteTest(eventStore);
    }

    [Test]
    public async Task GIVEN_events_to_save_with_existing_concurrency_WHEN_executing_calling_store_methods_THEN_throws_ConcurrencyException()
    {
        var eventStore = new SqlServerEventStore<string>(Settings, SqlStatementsLoader, new SqlServerConnectionFactory(), MockEventDataSerializer.Object);

        await ExecuteConcurrencyCheckTest(eventStore);
    }
}