using System.Threading;
using System.Threading.Tasks;
using EzDomain.EventSourcing.Domain.Model;

namespace EzDomain.EventSourcing.Domain.Repositories
{
    public interface IRepository<TAggregateRoot, in TAggregateRootId>
        where TAggregateRoot : class, IAggregateRoot<TAggregateRootId>
        where TAggregateRootId : class, IAggregateRootId
    {
        Task<TAggregateRoot> GetByIdAsync(string serializedId, CancellationToken cancellationToken = default);

        Task SaveAsync(TAggregateRoot aggregateRoot, string metadata = default, CancellationToken cancellationToken = default);
    }
}