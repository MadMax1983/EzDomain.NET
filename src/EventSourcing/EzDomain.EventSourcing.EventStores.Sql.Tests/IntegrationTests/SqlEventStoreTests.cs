using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.EventStores.Sql.Configuration;
using EzDomain.EventSourcing.EventStores.Sql.Data;
using EzDomain.EventSourcing.EventStores.Sql.Exceptions;
using EzDomain.EventSourcing.EventStores.Sql.Factories;
using EzDomain.EventSourcing.EventStores.Sql.Serializers;
using EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.IntegrationTests;

public abstract class SqlEventStoreTests<TDbConnectionFactory>
    where TDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _initialConnectionString;
    private readonly string _connectionString;

    protected SqlEventStoreTests(string initialConnectionString, string connectionString)
    {
        _initialConnectionString = initialConnectionString;
        _connectionString = connectionString;
    }

    protected EventStoreSettings Settings { get; } = new();

    protected ISqlStatementsLoader SqlStatementsLoader { get; } = new SqlStatementsLoader();

    protected Mock<IEventDataSerializer<string>> MockEventDataSerializer { get; }= new();
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        Settings.ConnectionStrings = new Dictionary<string, string>
        {
            {"EventStoreInitialize", _initialConnectionString},
            {"EventStore", _connectionString},
        };
            
        SqlStatementsLoader.LoadScripts();
        SqlStatementsLoader.LoadScripts("IntegrationTests\\Data\\SqlScripts");

        await CreateDatabaseAsync<SqlConnection>(Settings.ConnectionStrings["EventStoreInitialize"]);

        MockEventDataSerializer
            .Setup(m => m.Serialize(It.IsAny<Event>()))
            .Returns((Event @event) => JsonConvert.SerializeObject(@event));

        MockEventDataSerializer
            .Setup(m => m.Deserialize(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string jsonString, string type) =>
            {
                var jObj = JObject.Parse(jsonString);

                var version = jObj.GetValue("Version")!.Value<long>();

                var @event = (TestEvent)JsonConvert.DeserializeObject(jsonString, Type.GetType(type)!)!;

                @event.SetVersion(version);

                return @event;
            });
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await DropDatabaseAsync<SqlConnection>(Settings.ConnectionStrings["EventStoreInitialize"]);
    }

    protected async Task ExecuteConcurrencyCheckTest(IEventStore eventStore)
    {
        // Arrange
        var events = new List<TestEvent>
        {
            new TestEvent("arid", "sp1"),
            new TestEvent("arid", "sp2")
        };

        events[0].SetVersion(0);
        events[1].SetVersion(0);

        // Act
        Func<Task> act = async () => await eventStore.SaveAsync(events);

        // Assert
        await act
            .Should()
            .ThrowAsync<ConcurrencyException>();
    }

    protected async Task ExecuteTest(IEventStore eventStore)
    {
        // Arrange
        var events = new List<TestEvent>
        {
            new TestEvent("arid", "sp1"),
            new TestEvent("arid", "sp2"),
            new TestEvent("arid", "sp3")
        };

        for (var i = 0; i < events.Count; i++)
        {
            events[i].SetVersion(i);
        }

        // Act
        await eventStore.SaveAsync(events);

        var queriedEvents = await eventStore.GetByAggregateRootIdAsync("arid", Constants.InitialVersion);

        // Assert
        queriedEvents
            .Should()
            .BeEquivalentTo(events);

    }

    protected virtual async Task CreateDatabaseAsync<TDbConnection>(string connectionString)
        where TDbConnection : IDbConnection, new()
    {
        await ExecuteSqlStatementAsync<TDbConnection>(connectionString, SqlStatementsLoader["InitializeDatabase"]);
    }

    protected virtual async Task DropDatabaseAsync<TDbConnection>(string connectionString)
        where TDbConnection : IDbConnection, new()
    {
        await ExecuteSqlStatementAsync<TDbConnection>(connectionString, SqlStatementsLoader["DropDatabase"]);
    }

    protected virtual async Task ExecuteSqlStatementAsync<TDbConnection>(string connectionString, string sqlStatement)
        where TDbConnection : IDbConnection, new()
    {
        using var connection = new TDbConnection
        {
            ConnectionString = connectionString
        };

        await connection.ExecuteAsync(sqlStatement);
    }
}