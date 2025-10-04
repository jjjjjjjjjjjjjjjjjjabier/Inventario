using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class HistorialMovimientoService : IHistorialMovimientoService
    {
        private readonly IHistorialMovimientoRepository _repo;
        private readonly IEquipoComputoRepository _equipoRepo;

        public HistorialMovimientoService(IHistorialMovimientoRepository repo,
                                          IEquipoComputoRepository equipoRepo)
        {
            _repo = repo;
            _equipoRepo = equipoRepo;
        }

        public Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(int equipoId, CancellationToken ct = default)
            => _repo.ObtenerHistorialEquipoAsync(equipoId, ct);

        public async Task RegistrarMovimientoAsync(int equipoId, int? empleadoNuevoId, int? zonaNuevaId, string motivo, int usuarioResponsableId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));

            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct) ?? throw new InvalidOperationException("Equipo no encontrado.");

            var hist = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                FechaMovimiento = DateTime.UtcNow,
                EmpleadoAnteriorId = equipo.EmpleadoId,
                ZonaAnteriorId = equipo.ZonaId,
                AreaAnteriorId = equipo.AreaId,
                SedeAnteriorId = equipo.SedeId,
                EmpleadoNuevoId = empleadoNuevoId,
                ZonaNuevaId = zonaNuevaId,
                AreaNuevaId = equipo.AreaId,
                SedeNuevaId = equipo.SedeId,
                Motivo = motivo.Trim(),
                UsuarioResponsableId = usuarioResponsableId
            };

            await _repo.AgregarAsync(hist, ct);
        }
    }
}