using EzDomain.EventSourcing.Abstraction;

namespace EzDomain.EventSourcing.Tests.TestDoubles
{
    public sealed class BehaviorExecuted
        : Event
    {
        public BehaviorExecuted(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}