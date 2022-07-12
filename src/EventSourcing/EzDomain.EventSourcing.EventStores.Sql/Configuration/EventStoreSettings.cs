using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EzDomain.EventSourcing.EventStores.Sql.Configuration
{
    [ExcludeFromCodeCoverage]
    public sealed class EventStoreSettings
    {
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    }
}