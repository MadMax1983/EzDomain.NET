using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using EzDomain.EventSourcing.Exceptions;

namespace EzDomain.EventSourcing.Domain.Model
{
    public abstract class AggregateRoot<TId>
        : IAggregateRoot<TId>,
          IAggregateRootBehavior where TId : class, IAggregateRootId
    {
        private readonly List<Event> _changes;

        private readonly List<MethodInfo> _eventListenerMethods;

        private TId _id;

        /// <summary>
        /// Use this constructor only to restore aggregate root state from an event stream.
        /// </summary>
        protected AggregateRoot()
        {
            _changes = new List<Event>();
            
            _eventListenerMethods = InitializeEventListenerMethods();
            
            Version = Constants.InitialVersion;
        }

        /// <summary>
        /// Use this constructor only to create a new aggregate root.
        /// </summary>
        /// <param name="id">Aggregate Root Identifier</param>
        protected AggregateRoot(TId id)
            : this()
        {
            Id = id;
        }

        [SuppressMessage("ReSharper", "JoinNullCheckWithUsage", Justification = "Left with if statement for readability")]
        public TId Id
        {
            get => _id;
            protected set
            {
                if (_id is not null)
                {
                    throw new AggregateRootIdException("Aggregate root identifier has been already initialized.");
                }

                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _id = value;
            }
        }

        public long Version { get; private set; }

        /// <summary>
        /// Restores an aggregate root state from an event stream.
        /// </summary>
        /// <param name="eventStream">An event stream.</param>
        void IAggregateRootBehavior.RestoreFromStream(IReadOnlyCollection<Event> eventStream)
        {
            if (eventStream is null)
            {
                throw new EventStreamNullException(nameof(eventStream));
            }

            if (!eventStream.Any())
            {
                throw new EmptyEventStreamException("Aggregate root events stream is empty.");
            }

            var orderedEventStream = eventStream
                .OrderBy(e => e.Version)
                .ToList();

            orderedEventStream.ForEach(e => ApplyChange(e, false));

            var lastEvent = orderedEventStream.Last();

            Id = RestoreIdFromString(lastEvent.AggregateRootId);
            Version = lastEvent.Version;
        }

        IReadOnlyCollection<Event> IAggregateRootBehavior.GetUncommittedChanges()
        {
            return _changes.ToList();
        }

        void IAggregateRootBehavior.CommitChanges()
        {
            if (_id is null)
            {
                throw new AggregateRootIdException("Aggregate root identifier has not been initialized.");
            }

            var version = Version;

            _changes.ForEach(@event => @event.IncrementVersion(ref version));

            Version = version;

            _changes.Clear();
        }

        protected void ApplyChange(Event @event)
        {
            if (@event is null)
            {
                throw new EventNullException(nameof(@event));
            }

            ApplyChange(@event, true);
        }

        protected abstract TId RestoreIdFromString(string serializedId);

        private List<MethodInfo> InitializeEventListenerMethods()
        {
            return GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi => mi.Name == "On")
                .ToList();
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            InvokeEventListenerMethod(@event);

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        private void InvokeEventListenerMethod(Event @event)
        {
            var eventListenerMethod = _eventListenerMethods.SingleOrDefault(methodInfo => methodInfo.GetParameters().Single().ParameterType == @event.GetType());
            if (eventListenerMethod is null)
            {
                throw new MissingMethodException($"Event listener method was not found for the {@event.GetType().Name} event.");
            }

            eventListenerMethod.Invoke(this, new object[] { @event });
        }
    }
}