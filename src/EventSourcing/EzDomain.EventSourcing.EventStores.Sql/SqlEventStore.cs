using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.EventStores.Sql.Configuration;
using EzDomain.EventSourcing.EventStores.Sql.Data;
using EzDomain.EventSourcing.EventStores.Sql.Exceptions;
using EzDomain.EventSourcing.EventStores.Sql.Factories;
using EzDomain.EventSourcing.EventStores.Sql.Models;
using EzDomain.EventSourcing.EventStores.Sql.Serializers;

namespace EzDomain.EventSourcing.EventStores.Sql
{
    public abstract class SqlEventStore<TSerializationType>
        : IEventStore
    {
        protected SqlEventStore(
            EventStoreSettings settings,
            ISqlStatementsLoader sqlStatementsLoader,
            IDbConnectionFactory connectionFactory,
            IEventDataSerializer<TSerializationType> eventDataSerializer)
        {
            ConnectionString = settings.ConnectionStrings["EventStore"];

            SqlStatementsLoader = sqlStatementsLoader;
            ConnectionFactory = connectionFactory;

            EventDataSerializer = eventDataSerializer;
        }

        protected string ConnectionString { get; }

        protected ISqlStatementsLoader SqlStatementsLoader { get; }
        
        protected IDbConnectionFactory ConnectionFactory { get; }

        protected IEventDataSerializer<TSerializationType> EventDataSerializer { get; }

        public virtual async Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion, CancellationToken cancellationToken = default)
        {
            using var connection = ConnectionFactory.Create(ConnectionString);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            
            var inputParams = new
            {
                AggregateRootId = aggregateRootId,
                FromVersion = fromVersion
            };

            var sql = SqlStatementsLoader[nameof(GetByAggregateRootIdAsync)];

            var commandDefinition = new CommandDefinition(
                sql,
                inputParams,
                cancellationToken: cancellationToken);

            var eventEntities = await connection.QueryAsync<EventEntity<TSerializationType>>(commandDefinition);

            var events = eventEntities
                .Select(eventEntity => EventDataSerializer.Deserialize(eventEntity.Data!, eventEntity.Type!))
                .ToList();

            return events;
        }

        public virtual async Task SaveAsync(IReadOnlyCollection<Event> events, string? eventMetadata = default, CancellationToken cancellationToken = default)
        {
            IDbConnection connection = null!;
            IDbTransaction transaction = null!;

            try
            {
                connection = ConnectionFactory.Create(ConnectionString);
                
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                
                var sql = SqlStatementsLoader[nameof(SaveAsync)];

                var streamId = Guid.NewGuid().ToString();
                
                foreach (var @event in events)
                {
                    var eventEntity = new
                    {
                        Version = @event.Version,
                        AggregateRootId = @event.AggregateRootId,
                        StreamId = streamId,
                        TimeStampUtc = DateTime.UtcNow,
                        Type = @event.GetType().AssemblyQualifiedName,
                        Data = EventDataSerializer.Serialize(@event),
                        Metadata = eventMetadata
                    };
                    
                    var commandDefinition = new CommandDefinition(
                        sql,
                        eventEntity,
                        transaction,
                        cancellationToken: cancellationToken);

                    await connection.ExecuteAsync(commandDefinition);
                }
                
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (IsConcurrencyException(ex))
                {
                    throw new ConcurrencyException("A concurrency exception occured while saving event stream to the event store.", ex);
                }

                throw;
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        protected abstract bool IsConcurrencyException(Exception ex);
    }
}