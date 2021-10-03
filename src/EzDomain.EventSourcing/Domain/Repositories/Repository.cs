using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Factories;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.Exceptions;

namespace EzDomain.EventSourcing.Domain.Repositories
{
    public class Repository<TAggregateRoot, TAggregateRootId>
        : IRepository<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : class, IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        public Repository(IAggregateRootFactory<TAggregateRoot, TAggregateRootId> factory, IEventStore eventStore)
        {
            Factory = factory;
            
            EventStore = eventStore;
        }

        protected IAggregateRootFactory<TAggregateRoot, TAggregateRootId> Factory { get; }
        
        protected IEventStore EventStore { get; }

        public virtual async Task<TAggregateRoot> GetByIdAsync(string aggregateRootId, CancellationToken cancellationToken = default)
        {
            var eventStream = await EventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.InitialVersion, cancellationToken);
            if (!eventStream.Any())
            {
                return default;
            }

            var aggregateRoot = Factory.Create();
            var aggregateRootBehavior = CastToBehavior(aggregateRoot);

            aggregateRootBehavior.RestoreFromStream(eventStream);

            return aggregateRoot;
        }

        public virtual async Task SaveAsync(TAggregateRoot aggregateRoot, string metadata = default, CancellationToken cancellationToken = default)
        {
            if (aggregateRoot is null)
            {
                throw new AggregateRootNullException(nameof(aggregateRoot));
            }

            var aggregateRootBehavior = CastToBehavior(aggregateRoot);

            var changesToSave = aggregateRootBehavior.GetUncommittedChanges();
            if (!changesToSave.Any())
            {
                return;
            }

            aggregateRootBehavior.CommitChanges();

            await EventStore.SaveAsync(changesToSave, metadata, cancellationToken);
        }

        private static IAggregateRootBehavior CastToBehavior(TAggregateRoot aggregateRoot)
        {
            if (!(aggregateRoot is IAggregateRootBehavior aggregateRootBehavior))
            {
                throw new InvalidCastException("Aggregate root must implement AggregateRoot abstract class.");
            }

            return aggregateRootBehavior;
        }
    }
}