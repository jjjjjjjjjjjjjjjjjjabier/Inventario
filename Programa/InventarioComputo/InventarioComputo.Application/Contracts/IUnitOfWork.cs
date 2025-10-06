using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    // Abstracción de transacción agnóstica de EF
    public interface IAppTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }

    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}