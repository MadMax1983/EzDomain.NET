using System;
using AutoFixture.NUnit3;
using EzDomain.EventSourcing.Exceptions;
using EzDomain.EventSourcing.Tests.TestDoubles;
using FluentAssertions;
using NUnit.Framework;

namespace EzDomain.EventSourcing.Tests.UnitTests.Domain.Model
{
    [TestFixture]
    public sealed class EventTests
    {
        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_id_WHEN_creating_event_THEN_initializes_event_with_correct_state(string aggregateRootId)
        {
            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            // Assert
            @event.AggregateRootId
                .Should()
                .BeEquivalentTo(aggregateRootId);

            @event.Version
                .Should()
                .Be(Constants.InitialVersion);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void GIVEN_empty_aggregate_root_id_WHEN_creating_event_THEN_throws_AggregateRootIdException(string aggregateRootId)
        {
            // Act
            Action action = () => new BehaviorExecuted(aggregateRootId);

            // Assert
            action
                .Should()
                .Throw<AggregateRootIdException>();
        }

        [Test]
        [AutoData]
        public void GIVEN_aggregate_root_version_WHEN_incrementing_event_version_THEN_aggregate_root_version_is_incremented(string aggregateRootId)
        {
            // Arrange
            var aggregateRootVersion = Constants.InitialVersion;

            const long expectedAggregateRootVersion = Constants.InitialVersion + 1;

            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            @event.IncrementVersion(ref aggregateRootVersion);

            // Assert
            aggregateRootVersion
                .Should()
                .Be(expectedAggregateRootVersion);

            @event.Version
                .Should()
                .Be(expectedAggregateRootVersion);
        }

        [Test]
        [AutoData]
        public void GIVEN_invalid_aggregate_root_version_WHEN_incrementing_event_version_THEN_throws_AggregateRootVersionException(string aggregateRootId)
        {
            // Arrange
            long aggregateRootVersion = -2;

            // Act
            var @event = new BehaviorExecuted(aggregateRootId);

            var action = @event.Invoking(_ => @event.IncrementVersion(ref aggregateRootVersion));

            // Assert
            action
                .Should()
                .Throw<AggregateRootVersionException>();
        }
    }
}