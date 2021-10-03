using System.Diagnostics.CodeAnalysis;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Tests.TestDoubles
{
    [ExcludeFromCodeCoverage]
    public sealed class BehaviorExecuted
        : Event
    {
        public BehaviorExecuted(string aggregateRootId)
            : base(aggregateRootId)
        {
        }
    }
}