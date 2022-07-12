using System.Diagnostics.CodeAnalysis;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Tests.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class UnhandledBehaviorExecuted
    : Event
{
    public UnhandledBehaviorExecuted(string aggregateRootId)
        : base(aggregateRootId)
    {
    }
}