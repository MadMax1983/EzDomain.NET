using System;
using System.Collections.Generic;
using System.Linq;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.TestApp.EventSourcing.Events;

namespace EzDomain.EventSourcing.TestApp.Domain.Model
{
    internal class Order
        : AggregateRoot<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();

        private Address _deliveryAddress;
        private Address _billingAddress;

        private bool _orderPlaced;

        public Order()
        {
        }

        public Order(OrderId id)
            : base(id)
        {
        }

        public void AddOrderItem(string productId, decimal discount, int quantity)
        {
            if (_orderPlaced)
            {
                throw new InvalidOperationException("Order has been already placed.");
            }

            var @event = new ItemAddedToOrder(Id.ToString(), productId, discount, quantity);

            ApplyChange(@event);
        }

        public void ChangeDeliveryAddress(
            string country,
            string city,
            string county,
            string street,
            string buildingNumber,
            string flatNumber,
            string line1,
            string line2,
            bool isBillingAddressTheSame)
        {
            var deliveryAddressChanged = new DeliveryAddressChanged(
                Id.ToString(),
                country,
                city,
                county,
                street,
                buildingNumber,
                flatNumber,
                line1,
                line2);

            ApplyChange(deliveryAddressChanged);

            if (!isBillingAddressTheSame)
            {
                return;
            }

            ChangeBillingAddress(
                country,
                city,
                county,
                street,
                buildingNumber,
                flatNumber,
                line1,
                line2);
        }

        public void ChangeBillingAddress(
            string country,
            string city,
            string county,
            string street,
            string buildingNumber,
            string flatNumber,
            string line1,
            string line2)
        {
            var @event = new BillingAddressChanged(
                Id.ToString(),
                country,
                city,
                county,
                street,
                buildingNumber,
                flatNumber,
                line1,
                line2);

            ApplyChange(@event);
        }

        public void PlaceOrder()
        {
            if (_orderPlaced)
            {
                throw new InvalidOperationException("Order has been already placed.");
            }

            if (_orderItems.Count <= 0)
            {
                throw new InvalidOperationException("Order does not contain any items.");
            }

            if (_deliveryAddress is null)
            {
                throw new InvalidOperationException("No delivery address was specified in the order");
            }

            if (_billingAddress is null)
            {
                throw new InvalidOperationException("No billing address was specified in the order");
            }

            var @event = new OrderPlaced();

            ApplyChange(@event);
        }

        protected override OrderId RestoreIdFromString(string serializedId)
        {
            return new(serializedId);
        }

        private void On(ItemAddedToOrder @event)
        {
            var quantity = @event.Quantity;

            var orderItem = _orderItems.SingleOrDefault(oi => oi.ProductId == @event.ProductId);
            if (orderItem is not null)
            {
                quantity = orderItem.Quantity + @event.Quantity;

                _orderItems.Remove(orderItem);
            }

            orderItem = new OrderItem(@event.ProductId, @event.Discount, quantity);

            _orderItems.Add(orderItem);
        }

        private void On(DeliveryAddressChanged @event)
        {
            _deliveryAddress = new Address(
                @event.Country,
                @event.City,
                @event.County,
                @event.Street,
                @event.BuildingNumber,
                @event.FlatNumber,
                @event.Line1,
                @event.Line2);
        }

        private void On(BillingAddressChanged @event)
        {
            _billingAddress = new Address(
                @event.Country,
                @event.City,
                @event.County,
                @event.Street,
                @event.BuildingNumber,
                @event.FlatNumber,
                @event.Line1,
                @event.Line2);
        }

        private void On(OrderPlaced @event)
        {
            _orderPlaced = true;
        }
    }
}