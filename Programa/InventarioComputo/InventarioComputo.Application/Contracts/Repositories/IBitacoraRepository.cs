using InventarioComputo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IBitacoraRepository
    {
        Task<BitacoraEvento> AgregarAsync(BitacoraEvento evento, CancellationToken ct = default);
    }
}