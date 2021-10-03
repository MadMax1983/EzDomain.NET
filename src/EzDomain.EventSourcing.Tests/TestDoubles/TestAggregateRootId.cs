using System.Diagnostics.CodeAnalysis;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Tests.TestDoubles
{
    [ExcludeFromCodeCoverage]
    internal sealed class TestAggregateRootId
        : IAggregateRootId
    {
        private readonly string _value;

        public TestAggregateRootId(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}