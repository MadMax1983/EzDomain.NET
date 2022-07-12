using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.EventStores.InMemory.Tests.TestDoubles;

public sealed class TestEvent
    : Event
{
    public TestEvent(string aggregateRootId)
        : base(aggregateRootId)
    {
    }
}