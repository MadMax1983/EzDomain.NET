using System.Diagnostics.CodeAnalysis;

namespace EzDomain.EventSourcing.EventStores.Sql.Models
{
    [ExcludeFromCodeCoverage]
    internal sealed class EventEntity<TSerializationType>
    {
        public string? Type { get; set; }

        public TSerializationType? Data { get; set; }
    }
}