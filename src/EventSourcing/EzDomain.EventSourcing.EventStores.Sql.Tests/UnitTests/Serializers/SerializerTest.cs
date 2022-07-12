using System;
using System.Collections.Generic;
using EzDomain.EventSourcing.EventStores.Sql.Serializers;
using EzDomain.EventSourcing.EventStores.Sql.Tests.TestDoubles;
using FluentAssertions;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.UnitTests.Serializers;

public abstract class SerializerTest<TEventDataSerializationType>
{
    private const string StringProp1Value = "123";
    private const string StringProp2Value = "456";

    private readonly IEventDataSerializer<TEventDataSerializationType> _eventSerializer;

    protected SerializerTest(IEventDataSerializer<TEventDataSerializationType> eventSerializer)
    {
        _eventSerializer = eventSerializer;
    }

    protected void TestSerializer()
    {
        // Arrange
        var @event = new SerializationTestEvent(
            Guid.NewGuid().ToString(),
            StringProp1Value,
            123,
            new List<string> { "1", "2", "3" },
            new List<TestEventObject>
            {
                new TestEventObject("007"),
                new TestEventObject("005")
            },
            StringProp2Value);

        // Act
        var serializedEvent = _eventSerializer.Serialize(@event);

        var deserializedEvent = (SerializationTestEvent)_eventSerializer.Deserialize(serializedEvent, @event.GetType().AssemblyQualifiedName!);

        // Assert
        deserializedEvent.AggregateRootId
            .Should()
            .Be(@event.AggregateRootId);

        deserializedEvent.Version
            .Should()
            .Be(@event.Version);

        deserializedEvent.IntProp
            .Should()
            .Be(@event.IntProp);

        deserializedEvent.StringProp
            .Should()
            .Be(@event.StringProp);

        deserializedEvent.StringProp1
            .Should()
            .Be(@event.StringProp1);

        deserializedEvent.ObjCollection
            .Should()
            .HaveCount(@event.ObjCollection.Count);

        deserializedEvent.ObjCollection
            .Should()
            .BeEquivalentTo(@event.ObjCollection);

        deserializedEvent.StringCollection
            .Should()
            .HaveCount(@event.StringCollection.Count);

        deserializedEvent.StringCollection
            .Should()
            .BeEquivalentTo(@event.StringCollection);
    }
}