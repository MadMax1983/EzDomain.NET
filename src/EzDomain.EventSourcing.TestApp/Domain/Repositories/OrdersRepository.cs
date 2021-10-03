using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Factories;
using EzDomain.EventSourcing.Domain.Repositories;
using EzDomain.EventSourcing.TestApp.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Domain.Repositories
{
    internal sealed class OrdersRepository
        : Repository<Order, OrderId>
    {
        public OrdersRepository(IAggregateRootFactory<Order, OrderId> factory, IEventStore eventStore)
            : base(factory, eventStore)
        {
        }
    }
}