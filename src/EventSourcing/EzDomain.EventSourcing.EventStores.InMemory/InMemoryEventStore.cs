using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.Exceptions;

namespace EzDomain.EventSourcing.EventStores.InMemory
{
    public sealed class InMemoryEventStore
        : IEventStore
    {
        private static readonly Dictionary<EventKey, Event> Store = new();

        public Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(string aggregateRootId, long fromVersion, CancellationToken cancellationToken = default)
        {
            var result = Store
                .Where(e => e.Key.AggregateRootId.Equals(aggregateRootId))
                .OrderBy(e => e.Key.Version)
                .Select(e => e.Value)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Event>>(result);
        }

        public Task SaveAsync(IReadOnlyCollection<Event> events, string eventMetadata = default!, CancellationToken cancellationToken = default)
        {
            var transactionId = Guid.NewGuid();

            try
            {
                foreach (var @event in events)
                {
                    Store.Add(new EventKey(@event.AggregateRootId, @event.Version, transactionId), @event);
                }
            }
            catch (Exception ex)
            {
                var eventsToDelete = Store.Where(x => x.Key.TransactionId == transactionId);
                foreach (var eventToDelete in eventsToDelete)
                {
                    Store.Remove(eventToDelete.Key);
                }

                throw new ConcurrencyException(ex);
            }

            return Task.CompletedTask;
        }

        private sealed class EventKey
        {
            public EventKey(string aggregateRootId, long version, Guid transactionId)
            {
                AggregateRootId = aggregateRootId;
                Version = version;
                TransactionId = transactionId;
            }

            public string AggregateRootId { get; }

            public long Version { get; }

            public Guid TransactionId { get; }

            public override int GetHashCode()
            {
                return AggregateRootId.GetHashCode() | Version.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj is not EventKey eventKey)
                {
                    return false;
                }

                if (eventKey.AggregateRootId.Equals(AggregateRootId, StringComparison.Ordinal) &&
                    eventKey.Version == Version)
                {
                    return true;
                }

                return false;
            }
        }
    }
}