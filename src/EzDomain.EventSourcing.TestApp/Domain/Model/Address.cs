using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Domain.Model
{
    public sealed record Address(
            string Country,
            string City,
            string County,
            string Street,
            string BuildingNumber,
            string FlatNumber,
            string Line1,
            string Line2)
        : IValueObject;
}