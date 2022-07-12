using EzDomain.EventSourcing.EventStores.Sql.Serializers;
using NUnit.Framework;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.UnitTests.Serializers;

[TestFixture]
public sealed class JsonEventDataSerializerTests
    : SerializerTest<string>
{
    public JsonEventDataSerializerTests()
        : base(new JsonEventDataSerializer())
    {
            
    }

    [Test]
    public void GIVEN_event_WHEN_serializing_AND_deserializing_THEN_original_AND_serialized_event_data_are_equal()
    {
        TestSerializer();
    }
}