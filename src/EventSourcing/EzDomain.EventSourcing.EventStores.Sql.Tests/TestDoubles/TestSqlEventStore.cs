using System;
using EzDomain.EventSourcing.EventStores.Sql.Configuration;
using EzDomain.EventSourcing.EventStores.Sql.Data;
using EzDomain.EventSourcing.EventStores.Sql.Factories;
using EzDomain.EventSourcing.EventStores.Sql.Serializers;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;

internal sealed class TestSqlEventStore<TSerializationType>
    : SqlEventStore<TSerializationType>
{
    public TestSqlEventStore(
        EventStoreSettings settings,
        ISqlStatementsLoader sqlStatementsLoader,
        IDbConnectionFactory connectionFactory,
        IEventDataSerializer<TSerializationType> eventDataSerializer)
        : base(
            settings,
            sqlStatementsLoader,
            connectionFactory,
            eventDataSerializer)
    {
    }

    protected override bool IsConcurrencyException(Exception ex)
    {
        return false;
    }
}