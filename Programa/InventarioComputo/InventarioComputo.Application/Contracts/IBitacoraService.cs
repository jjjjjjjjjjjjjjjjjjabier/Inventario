using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IBitacoraService
    {
        Task RegistrarAsync(string entidad, string accion, int entidadId, int? usuarioResponsableId, string? detalles = null, CancellationToken ct = default);
    }
}