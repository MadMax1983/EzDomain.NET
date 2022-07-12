using System;
using System.Reflection;
using EzDomain.EventSourcing.Domain.Model;
using Newtonsoft.Json;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;

[Serializable]
internal sealed class TestEvent
    : Event
{
    public TestEvent()
    {
    }

    [JsonConstructor]
    public TestEvent(string aggregateRootId, string stringProp)
        : base(aggregateRootId)
    {
        StringProp = stringProp;
    }

    public string StringProp { get; } = null!;

    public void SetVersion(long version)
    {
        var versionField = typeof(Event).GetField("_version", BindingFlags.Instance | BindingFlags.NonPublic);
        if (versionField != null)
        {
            versionField.SetValue(this, version);
        }
    }
}