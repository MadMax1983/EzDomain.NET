using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using EzDomain.EventSourcing.Domain.Model;
using EzDomain.EventSourcing.Exceptions;
using EzDomain.EventSourcing.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace EzDomain.EventSourcing.Tests.UnitTests.Domain.Model;

[TestFixture]
public sealed class AggregateRootTests
{
    [Test]
    [AutoData]
    public void GIVEN_serialized_aggregate_root_id_WHEN_instantiating_aggregate_root_THEN_initializes_aggregate_root_with_correct_state(string serializedAggregateRootId)
    {
        // Act
        var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
        var aggregateRoot = new TestAggregateRoot(aggregateRootId);
            
        // Assert
        aggregateRoot.Id
            .Should()
            .NotBeNull();

        aggregateRoot.Version
            .Should()
            .Be(Constants.InitialVersion);
    }

    [Test]
    [AutoData]
    public void GIVEN_aggregate_root_stream_WHEN_restoring_from_events_stream_THEN_sets_aggregate_root_correct_state(string serializedAggregateRootId)
    {
        // Arrange
        const int expectedNumberOfUncommittedEvents = 0;
            
        var eventVersion = Constants.InitialVersion;

        var @event = new BehaviorExecuted(serializedAggregateRootId);
        @event.IncrementVersion(ref eventVersion);

        var eventsStream = new List<Event>
        {
            @event
        };

        var aggregateRoot = new TestAggregateRoot();
        var aggregateRootBehavior = (IAggregateRootBehavior)aggregateRoot;

        // Act
        // TODO: Consider method change name to RestoreFromEventsStream
        aggregateRootBehavior.RestoreFromStream(eventsStream);

        // Assert
        aggregateRoot.Id.ToString()
            .Should()
            .Be(serializedAggregateRootId);

        aggregateRoot.Version
            .Should()
            .Be(eventVersion)
            .And
            .BeGreaterThan(Constants.InitialVersion);

        aggregateRootBehavior.GetUncommittedChanges().Count
            .Should()
            .Be(expectedNumberOfUncommittedEvents);
    }

    [Test]
    public void GIVEN_no_aggregate_root_events_stream_WHEN_restoring_from_events_stream_THEN_throws_EventStreamNullException()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot();

        // Act
        var act = aggregateRoot.Invoking(ar => ((IAggregateRootBehavior)ar).RestoreFromStream(null));

        // Assert
        act.Should()
            .Throw<EventStreamNullException>();

        aggregateRoot.Id
            .Should()
            .BeNull();

        aggregateRoot.Version
            .Should()
            .Be(Constants.InitialVersion);
    }

    [Test]
    public void GIVEN_empty_aggregate_root_events_stream_WHEN_restoring_from_events_stream_THEN_throws_EmptyEventStreamException()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot();

        // Act
        var act = aggregateRoot.Invoking(ar => ((IAggregateRootBehavior)ar).RestoreFromStream(Array.Empty<Event>()));

        // Assert
        act.Should()
            .Throw<EmptyEventStreamException>();

        aggregateRoot.Id
            .Should()
            .BeNull();

        aggregateRoot.Version
            .Should()
            .Be(Constants.InitialVersion);
    }

    [Test]
    [AutoData]
    public void GIVEN_aggregate_root_with_uncommitted_changes_WHEN_commiting_changes_THEN_sets_correct_aggregate_root_version(string serializedAggregateRootId)
    {
        // Arrange
        const int expectedNumberOfChangesToSave = 1;
        const int expectedVersion = 0;
        const int expectedNumberOfUncommittedChanges = 0;

        var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
        var aggregateRoot = new TestAggregateRoot(aggregateRootId);
        var aggregateRootBehavior = (IAggregateRootBehavior)aggregateRoot;

        // Act
        aggregateRoot.ExecuteBehavior();

        // Assert
        var changesToSave = aggregateRootBehavior.GetUncommittedChanges().ToArray();

        aggregateRootBehavior.CommitChanges();

        for (var i = 0; i < changesToSave.Length; i++)
        {
            changesToSave[i].Version
                .Should()
                .Be(i);
        }

        changesToSave.Length
            .Should()
            .Be(expectedNumberOfChangesToSave);

        aggregateRoot.Version
            .Should()
            .Be(expectedVersion);

        aggregateRootBehavior.GetUncommittedChanges().Count
            .Should()
            .Be(expectedNumberOfUncommittedChanges);
    }

    [Test]
    public void GIVEN_aggregate_root_without_id_WHEN_commiting_changes_THEN_throws_AggregateRootIdException()
    {
        // Arrange
        var aggregateRoot = new TestAggregateRoot();

        // Act
        var act = aggregateRoot.Invoking(ar => ((IAggregateRootBehavior)ar).CommitChanges());

        // Assert
        act.Should()
            .Throw<AggregateRootIdException>();

        aggregateRoot.Version
            .Should()
            .Be(Constants.InitialVersion);
    }

    [Test]
    [AutoData]
    public void GIVEN_event_without_aggregate_root_event_handler_implemented_WHEN_processing_event_within_aggregate_root_THEN_throws_MissingMethodException(string serializedAggregateRootId)
    {
        // Arrange
        var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
        var aggregateRoot = new TestAggregateRoot(aggregateRootId);

        // Act
        var act = aggregateRoot.Invoking(ar => ar.ExecuteUnhandledBehavior());
            
        // Assert
        act.Should()
            .Throw<MissingMethodException>();
    }

    [Test]
    [AutoData]
    public void GIVEN_aggregate_root_id_WHEN_aggregate_root_id_is_already_initialized_THEN_throws_AggregateRootIdException(string serializedAggregateRootId)
    {
        // Arrange
        var aggregateRootId = new TestAggregateRootId(serializedAggregateRootId);
        var aggregateRoot = new TestAggregateRoot(aggregateRootId);
            
        // Act
        var act = aggregateRoot.Invoking(ar => ar.SetId(serializedAggregateRootId));
            
        // Assert
        act.Should()
            .Throw<AggregateRootIdException>();
    }
}