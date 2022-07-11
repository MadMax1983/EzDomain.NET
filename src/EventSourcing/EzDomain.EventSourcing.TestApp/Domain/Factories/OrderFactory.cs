using EzDomain.EventSourcing.Domain.Factories;
using EzDomain.EventSourcing.TestApp.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Domain.Factories
{
    internal sealed class OrderFactory
        : AggregateRootFactory<Order, OrderId>
    {
        public Order Create(OrderId id) => new(id);
    }
}