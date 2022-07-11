using System.Diagnostics.CodeAnalysis;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Tests.TestDoubles
{
    [ExcludeFromCodeCoverage]
    internal sealed class TestAggregateRoot
        : AggregateRoot<TestAggregateRootId>, ITestAggregateRoot
    {
        ///<inheritdoc/>
        public TestAggregateRoot()
        {
        }

        ///<inheritdoc/>
        public TestAggregateRoot(TestAggregateRootId id)
            : base(id)
        {
        }

        public void SetId(string id)
        {
            Id = new TestAggregateRootId(id);
        }

        public void ExecuteBehavior()
        {
            ApplyChange(new BehaviorExecuted(Id.ToString()));
        }

        public void ExecuteUnhandledBehavior()
        {
            ApplyChange(new UnhandledBehaviorExecuted(Id.ToString()));
        }

        protected override TestAggregateRootId RestoreIdFromString(string serializedId)
        {
            return new(serializedId);
        }

        private void On(BehaviorExecuted @event)
        {
        }
    }
}