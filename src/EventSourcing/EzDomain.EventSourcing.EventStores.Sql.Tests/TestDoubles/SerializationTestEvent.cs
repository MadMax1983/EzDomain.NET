using System;
using System.Collections.Generic;
using EzDomain.EventSourcing.Domain.Model;
using Newtonsoft.Json;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;

[Serializable]
internal sealed class SerializationTestEvent
    : Event
{
    [JsonConstructor]
    public SerializationTestEvent(
        string aggregateRootId,
        string stringProp,
        int intProp,
        IReadOnlyCollection<string> stringCollection,
        IReadOnlyCollection<TestEventObject> objCollection,
        string stringProp1)
        : base(aggregateRootId)
    {
        StringProp = stringProp;
        StringProp1 = stringProp1;

        IntProp = intProp;

        StringCollection = stringCollection;
        ObjCollection = objCollection;
    }

    public string StringProp { get; }

    public string StringProp1 { get; }

    public int IntProp { get; }

    public IReadOnlyCollection<string> StringCollection { get; }

    public IReadOnlyCollection<TestEventObject> ObjCollection { get; }
}