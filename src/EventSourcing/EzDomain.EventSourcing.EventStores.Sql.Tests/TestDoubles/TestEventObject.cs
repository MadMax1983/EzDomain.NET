using System;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;

[Serializable]
internal sealed class TestEventObject
{
    public TestEventObject(string objString)
    {
        ObjString = objString;
    }

    public string ObjString { get; }
}