using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.EventSourcing.EventStores
{
    internal sealed class InProcEventStore
        : IEventStore
    {
        private static readonly IDictionary<string, Event> Events = new Dictionary<string, Event>();

        public Task<IReadOnlyCollection<Event>> GetByAggregateRootIdAsync(
            string aggregateRootId,
            long fromVersion,
            CancellationToken cancellationToken = default)
        {
            var events = Events
                .Where(@event =>
                {
                    var keyElements = @event.Key.Split('_');

                    var eventAggregateRootId = keyElements[0];
                    var eventVersion = keyElements[1];

                    return aggregateRootId == eventAggregateRootId &&
                           long.Parse(eventVersion) >= fromVersion;
                })
                .Select(@event => @event.Value)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Event>>(events);
        }

        public Task SaveAsync(
            IReadOnlyCollection<Event> events,
            string eventMetadata = default,
            CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                Events.Add($"{@event.AggregateRootId}_{@event.Version}", @event);
            }

            return Task.FromResult(0);
        }
    }
}