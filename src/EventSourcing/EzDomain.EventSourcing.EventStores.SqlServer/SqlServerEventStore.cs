using System;
using System.Data.SqlClient;
using EzDomain.EventSourcing.EventStores.Sql;
using EzDomain.EventSourcing.EventStores.Sql.Configuration;
using EzDomain.EventSourcing.EventStores.Sql.Data;
using EzDomain.EventSourcing.EventStores.Sql.Factories;
using EzDomain.EventSourcing.EventStores.Sql.Serializers;

namespace EzDomain.EventSourcing.EventStores.SqlServer
{
    public sealed class SqlServerEventStore<TSerializationType>
        : SqlEventStore<TSerializationType>
    {
        public SqlServerEventStore(
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
            const int primaryKeyViolationCode = 2627;
            const int uniqueIndexViolationCode = 2601;

            return ex is SqlException {Number: primaryKeyViolationCode or uniqueIndexViolationCode};
        }
    }
}