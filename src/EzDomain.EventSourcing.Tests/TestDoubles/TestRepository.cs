using EzDomain.EventSourcing.Domain.EventStores;
using EzDomain.EventSourcing.Domain.Factories;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.Domain.Repositories;

namespace EzDomain.EventSourcing.Tests.TestDoubles
{
    internal sealed class TestRepository
        : Repository<ITestAggregateRoot, IAggregateRootId>
    {
        public TestRepository(IAggregateRootFactory<ITestAggregateRoot, IAggregateRootId> factory, IEventStore eventStore)
            : base(factory, eventStore)
        {
        }
    }
}