using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IUnidadService
    {
        // CORRECCIÓN 1: El tipo de retorno se cambia a IReadOnlyList para seguir las buenas prácticas.
        Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);

        Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct = default);

        Task EliminarAsync(int id, CancellationToken ct = default);

        // CORRECCIÓN 2: Se añaden los nuevos métodos de validación que faltaban.
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir, CancellationToken ct = default);

        Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? idExcluir, CancellationToken ct = default);
    }
}