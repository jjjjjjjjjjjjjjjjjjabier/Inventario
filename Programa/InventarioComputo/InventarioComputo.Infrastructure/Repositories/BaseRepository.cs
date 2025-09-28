using InventarioComputo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace InventarioComputo.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly InventarioDbContext _context;

        public BaseRepository(InventarioDbContext context)
        {
            _context = context;
        }

        protected IQueryable<T> GetQuery(bool asNoTracking = true)
        {
            var query = _context.Set<T>().AsQueryable();
            return asNoTracking ? query.AsNoTracking() : query;
        }
        
        protected async Task SaveChangesWithValidationAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("circular") ?? false)
            {
                throw new InvalidOperationException("Se detectó una dependencia circular en los datos. " +
                    "Revise las relaciones entre entidades.", ex);
            }
        }
    }
}