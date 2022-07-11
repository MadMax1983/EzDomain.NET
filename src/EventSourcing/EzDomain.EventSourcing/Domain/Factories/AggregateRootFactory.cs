using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Domain.Factories
{
    // TODO: Add unit tests.
    public class AggregateRootFactory<TAggregateRoot, TAggregateRootId>
        : IAggregateRootFactory<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : class, IAggregateRoot<TAggregateRootId>, new()
        where TAggregateRootId : class, IAggregateRootId
    {
        /// <summary>
        /// Use this method only to restore aggregate root state from an event stream.
        /// </summary>
        /// <returns>Aggregate root</returns>
        public virtual TAggregateRoot Create()
        {
            return new TAggregateRoot();
        }
    }
}