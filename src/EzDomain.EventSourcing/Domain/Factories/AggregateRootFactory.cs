using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Domain.Factories
{
    // TODO: Add unit tests.
    public class AggregateRootFactory<TAggregateRoot, TAggregateRootId>
        : IAggregateRootFactory<TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : class, IAggregateRoot<TAggregateRootId>, new()
        where TAggregateRootId : class, IAggregateRootId
    {
        public virtual TAggregateRoot Create()
        {
            return new TAggregateRoot();
        }
    }
}