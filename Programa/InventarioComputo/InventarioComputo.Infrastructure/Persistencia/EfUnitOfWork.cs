using InventarioComputo.Application.Contracts;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Infrastructure.Persistencia
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly InventarioDbContext _context;

        public EfUnitOfWork(InventarioDbContext context)
        {
            _context = context;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}