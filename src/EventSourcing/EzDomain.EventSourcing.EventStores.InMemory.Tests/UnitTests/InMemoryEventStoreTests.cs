using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.EventStores.InMemory.Tests.TestDoubles;
using EzDomain.EventSourcing.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace EzDomain.EventSourcing.EventStores.InMemory.Tests.UnitTests;

[TestFixture]
public sealed class InMemoryEventStoreTests
{
    [Test]
    public async Task GIVEN_two_event_streams_with_events_with_same_aggregate_root_identifier_and_version_WHEN_saving_events_THEN_throws_ConcurrencyException_and_rollback_changes()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();

        var aggregateRootId = Guid.NewGuid().ToString();

        var eventStream1 = new List<Event>
        {
            CreateAggregateRoot(aggregateRootId, 0)
        };


        var eventStream2 = new List<Event>
        {
            CreateAggregateRoot(aggregateRootId, 1),
            CreateAggregateRoot(aggregateRootId, 0)
            
        };

        // Act
        await eventStore.SaveAsync(eventStream1, string.Empty, CancellationToken.None);

        var action = async () =>
        {
            await eventStore.SaveAsync(eventStream2, string.Empty, CancellationToken.None);
        };

        // Assert
        await action
            .Should()
            .ThrowAsync<ConcurrencyException>();

        var selectedEvents = await eventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.InitialVersion, CancellationToken.None);
        
        selectedEvents.Count
            .Should()
            .Be(1);
    }

    [Test]
    public async Task GIVEN_event_stream_with_events_with_same_aggregate_root_identifier_and_version_WHEN_saving_events_THEN_throws_ConcurrencyException_and_rollback_changes()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();

        var aggregateRootId = Guid.NewGuid().ToString();


        var eventStream = new List<Event>
        {
            CreateAggregateRoot(aggregateRootId, 0),
            CreateAggregateRoot(aggregateRootId, 0)
            
        };

        // Act
        var action = async () =>
        {
            await eventStore.SaveAsync(eventStream, string.Empty, CancellationToken.None);
        };

        // Assert
        await action
            .Should()
            .ThrowAsync<ConcurrencyException>();

        var selectedEvents = await eventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.InitialVersion, CancellationToken.None);
        
        selectedEvents.Count
            .Should()
            .Be(0);
    }

    [Test]
    public async Task GIVEN_event_stream_with_events_with_same_aggregate_root_identifier_and_different_version_WHEN_saving_events_THEN_saves_events_in_event_store()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();

        var aggregateRootId = Guid.NewGuid().ToString();


        var eventStream = new List<Event>
        {
            CreateAggregateRoot(aggregateRootId, 0),
            CreateAggregateRoot(aggregateRootId, 1)
            
        };

        // Act
        await eventStore.SaveAsync(eventStream, string.Empty, CancellationToken.None);
        
        var selectedEvents = await eventStore.GetByAggregateRootIdAsync(aggregateRootId, Constants.InitialVersion, CancellationToken.None);
        
        // Assert

        selectedEvents.Count
            .Should()
            .Be(2);
    }

    [Test]
    public async Task GIVEN_two_event_store_instances_WHEN_saving_and_getting_events_THEN_saves_and_returns_events_from_event_store()
    {
        // Arrange
        var eventStore1 = new InMemoryEventStore();
        var eventStore2 = new InMemoryEventStore();

        var aggregateRootId = Guid.NewGuid().ToString();


        var eventStream = new List<Event>
        {
            CreateAggregateRoot(aggregateRootId, 0),
            CreateAggregateRoot(aggregateRootId, 1)
            
        };

        // Act
        await eventStore1.SaveAsync(eventStream, string.Empty, CancellationToken.None);
        
        var selectedEvents = await eventStore2.GetByAggregateRootIdAsync(aggregateRootId, Constants.InitialVersion, CancellationToken.None);
        
        // Assert

        selectedEvents.Count
            .Should()
            .Be(2);
    }

    private static Event CreateAggregateRoot(string aggregateRootId, long newVersion)
    {
        var @event = new TestEvent(aggregateRootId);

        var eventVersion = @event.GetType().BaseType!.GetField("_version",  BindingFlags.Instance | BindingFlags.NonPublic)!;

        eventVersion.SetValue(@event, newVersion);

        return @event;
    }
}