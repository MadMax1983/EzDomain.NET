using System;
using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.TestApp.Domain.Factories;
using EzDomain.EventSourcing.TestApp.Domain.Model;
using EzDomain.EventSourcing.TestApp.Domain.Repositories;
using EzDomain.EventSourcing.TestApp.EventSourcing.EventStores;

namespace EzDomain.EventSourcing.TestApp
{
    public static class Program
    {
        private static readonly OrderFactory OrderFactory = new();
        private static readonly InProcEventStore EventStore = new();
        private static readonly OrdersRepository OrdersRepository = new(OrderFactory, EventStore);

        public static async Task Main(string[] args)
        {
            // Creates product ids
            var appleId = Guid.NewGuid().ToString();
            var orangeId = Guid.NewGuid().ToString();

            // Creates order id
            var orderIdString = Guid.NewGuid().ToString();
            var orderId = new OrderId(orderIdString);

            // Gets order from the repository or creates new order if does not exist
            var order = await OrdersRepository.GetByIdAsync(orderIdString, CancellationToken.None) ?? OrderFactory.Create(orderId);

            // Adds apples to the order
            order.AddOrderItem(appleId, 0, 2);

            // Saves order in the repository
            await OrdersRepository.SaveAsync(order, null, CancellationToken.None);

            // Gets order from the repository
            order = await OrdersRepository.GetByIdAsync(orderIdString, CancellationToken.None);

            // Updates apples quantity and oranges to the order
            order.AddOrderItem(appleId, 5, 1);
            order.AddOrderItem(orangeId, 0, 2);

            // Add Delivery and Billing address
            order.ChangeDeliveryAddress(
                "PL",
                "Warsaw",
                "Maz.",
                "Marszałkowska",
                "1",
                "2",
                null,
                null,
                true);

            // Places order
            order.PlaceOrder();

            // Saves order in the repository
            await OrdersRepository.SaveAsync(order, null, CancellationToken.None);

            Console.ReadKey();
        }
    }
}