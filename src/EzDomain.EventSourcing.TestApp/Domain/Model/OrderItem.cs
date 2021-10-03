using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Domain.Model
{
    internal sealed record OrderItem(string ProductId, decimal Discount, int Quantity)
        : IValueObject;
}