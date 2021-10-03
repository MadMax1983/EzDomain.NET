using System;

namespace EzDomain.EventSourcing.TestApp.Events
{
    [Serializable]
    public sealed class DeliveryAddressChanged
        : AddressChanged
    {
        /// <inheritdoc />
        public DeliveryAddressChanged()
        {
        }

        /// <inheritdoc />
        public DeliveryAddressChanged(
            string orderId,
            string country,
            string city,
            string county,
            string street,
            string buildingNumber,
            string flatNumber,
            string line1,
            string line2)
            : base(
                orderId,
                country,
                city,
                county,
                street,
                buildingNumber,
                flatNumber,
                line1,
                line2)
        {
        }
    }
}