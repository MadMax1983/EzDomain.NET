using System.Collections.Generic;

namespace EzDomain.EventSourcing.Domain.Model
{
    internal interface IAggregateRootBehavior
    {
        void RestoreFromStream(IReadOnlyCollection<Event> eventStream);

        IReadOnlyCollection<Event> GetUncommittedChanges();

        void CommitChanges();
    }
}