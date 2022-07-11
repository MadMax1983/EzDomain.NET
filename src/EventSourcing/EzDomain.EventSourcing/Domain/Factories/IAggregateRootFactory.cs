using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Domain.Factories
{
    public interface IAggregateRootFactory<out TAggregateRoot, TAggregateRootId>
        where TAggregateRoot : IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        TAggregateRoot Create();
    }
}