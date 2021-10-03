using System;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.TestApp.Events
{
    [Serializable]
    public abstract class AddressChanged
        : Event
    {
        /// <inheritdoc />
        protected AddressChanged()
        {
        }

        /// <inheritdoc />
        protected AddressChanged(
            string orderId,
            string country,
            string city,
            string county,
            string street,
            string buildingNumber,
            string flatNumber,
            string line1,
            string line2)
            : base(orderId)
        {
            Country = country;
            City = city;
            County = county;
            Street = street;
            BuildingNumber = buildingNumber;
            FlatNumber = flatNumber;
            Line1 = line1;
            Line2 = line2;
        }

        public string Country { get; }

        public string City { get; }

        public string County { get; }

        public string Street { get; }

        public string BuildingNumber { get; }

        public string FlatNumber { get; }

        public string Line1 { get; }

        public string Line2 { get; }
    }
}