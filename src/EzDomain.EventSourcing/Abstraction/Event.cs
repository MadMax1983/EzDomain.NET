using System;
using EzDomain.EventSourcing.Exceptions;

namespace EzDomain.EventSourcing.Abstraction
{
    /// <summary>
    /// Base class for domain events created by an aggregate root.
    /// </summary>
    [Serializable]
    public abstract class Event
    {
        private long _version;

        /// <summary>
        /// Use this constructor only for deserialization form an event store.
        /// </summary>
        protected Event()
        {
            _version = Constants.InitialVersion;
        }

        /// <summary>
        /// Use this constructor only for creation of a new event.
        /// </summary>
        /// <param name="aggregateRootId">Serialized to string aggregate root identifier.</param>
        /// <exception cref="AggregateRootIdException">Thrown if serialized to string aggregate root identifier is null, empty or whitespace.</exception>
        protected Event(string aggregateRootId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(aggregateRootId))
            {
                throw new AggregateRootIdException("Serialized to string aggregate root identifier cannot be null, empty or whitespace.");
            }

            AggregateRootId = aggregateRootId;
        }

        /// <summary>
        /// Serialized to string aggregate root identifier.
        /// </summary>
        public string AggregateRootId { get; }

        /// <summary>
        /// Event version.
        /// </summary>
        public long Version => _version;

        /// <summary>
        /// Increments version of the event to the correct sequence state.
        /// </summary>
        /// <param name="aggregateRootVersion">Current aggregate root version</param>
        /// <exception cref="AggregateRootVersionException">Thrown if provided aggregate root version is less than -1 (initial aggregate root version).</exception>
        internal void IncrementVersion(ref long aggregateRootVersion)
        {
            if (aggregateRootVersion < Constants.InitialVersion)
            {
                throw new AggregateRootVersionException($"Aggregate root version must be greater or equal to {Constants.InitialVersion} but was {aggregateRootVersion}.");
            }

            _version = ++aggregateRootVersion;
        }
    }
}