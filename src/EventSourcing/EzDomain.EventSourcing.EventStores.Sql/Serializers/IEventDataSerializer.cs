using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.EventStores.Sql.Serializers
{
    public interface IEventDataSerializer<TEventDataSerializationType>
    {
        TEventDataSerializationType Serialize<TEvent>(TEvent @event)
            where TEvent : Event;

        Event Deserialize(TEventDataSerializationType obj, string type);
    }
}