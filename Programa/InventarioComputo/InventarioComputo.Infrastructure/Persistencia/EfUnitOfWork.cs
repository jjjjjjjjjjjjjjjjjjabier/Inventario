using InventarioComputo.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Infrastructure.Persistencia
{
    internal sealed class EfTransaction : IAppTransaction
    {
        private readonly IDbContextTransaction _tx;
        public EfTransaction(IDbContextTransaction tx) => _tx = tx;

        public Task CommitAsync(CancellationToken ct = default) => _tx.CommitAsync(ct);
        public Task RollbackAsync(CancellationToken ct = default) => _tx.RollbackAsync(ct);
        public ValueTask DisposeAsync() => _tx.DisposeAsync();
    }

    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly InventarioDbContext _ctx;

        public EfUnitOfWork(InventarioDbContext ctx) => _ctx = ctx;

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _ctx.SaveChangesAsync(cancellationToken) > 0;

        public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var tx = await _ctx.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(tx);
        }
    }
}