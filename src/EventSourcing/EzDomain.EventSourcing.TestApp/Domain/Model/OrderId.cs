using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Domain.Model
{
    internal sealed record OrderId
        : IAggregateRootId
    {
        private readonly string _value;

        public OrderId(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}