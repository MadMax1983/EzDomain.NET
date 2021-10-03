using System;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Events
{
    [Serializable]
    public sealed class ItemAddedToOrder
        : Event
    {
        /// <inheritdoc />
        internal ItemAddedToOrder()
        {
        }

        /// <inheritdoc />
        internal ItemAddedToOrder(string orderId, string productId, decimal discount, int quantity)
            : base(orderId)
        {
            ProductId = productId;

            Discount = discount;

            Quantity = quantity;
        }

        public string ProductId { get; }

        public decimal Discount { get; }

        public int Quantity { get; }
    }
}