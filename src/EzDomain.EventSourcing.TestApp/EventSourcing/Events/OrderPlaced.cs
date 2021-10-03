using System;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Events
{
    [Serializable]
    public sealed class OrderPlaced
        : Event
    {
        /// <inheritdoc />
        public OrderPlaced()
        {
        }

        /// <inheritdoc />
        public OrderPlaced(string orderId)
            : base(orderId)
        {
        }
    }
}