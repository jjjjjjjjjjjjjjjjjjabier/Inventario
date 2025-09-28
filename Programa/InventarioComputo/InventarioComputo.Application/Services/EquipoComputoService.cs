using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class EquipoComputoService : IEquipoComputoService
    {
        private readonly IEquipoComputoRepository _repo;

        public EquipoComputoService(IEquipoComputoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default)
        {
            return await _repo.ObtenerTodosAsync(incluirInactivos, ct);
        }

        public async Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _repo.ObtenerPorIdAsync(id, ct);
        }

        public async Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            // Validar que no exista otro equipo con el mismo número de serie
            if (await _repo.ExistsByNumeroSerieAsync(equipo.NumeroSerie, null, ct))
            {
                throw new InvalidOperationException($"Ya existe un equipo con el número de serie '{equipo.NumeroSerie}'.");
            }

            return await _repo.AgregarAsync(equipo, ct);
        }

        public async Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            // Validar que no exista otro equipo con el mismo número de serie
            if (await _repo.ExistsByNumeroSerieAsync(equipo.NumeroSerie, equipo.Id, ct))
            {
                throw new InvalidOperationException($"Ya existe otro equipo con el número de serie '{equipo.NumeroSerie}'.");
            }

            await _repo.ActualizarAsync(equipo, ct);
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            await _repo.EliminarAsync(id, ct);
        }
    }
}