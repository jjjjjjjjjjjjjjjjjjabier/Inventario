using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task<T> UpdateAsync(T entity, CancellationToken ct = default);

        // Devuelve la abstracción de transacción propia de la capa Application
        Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}