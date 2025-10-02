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

        public HistorialMovimientoService(
            IHistorialMovimientoRepository repo,
            IEquipoComputoRepository equipoRepo)
        {
            _repo = repo;
            _equipoRepo = equipoRepo;
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(
            int equipoId, CancellationToken ct = default)
        {
            return await _repo.ObtenerHistorialEquipoAsync(equipoId, ct);
        }

        public async Task RegistrarMovimientoAsync(
            int equipoId, int? usuarioNuevoId, int? zonaNuevaId, string motivo,
            int usuarioResponsableId, CancellationToken ct = default)
        {
            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}");

            var movimiento = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                FechaMovimiento = DateTime.Now,
                // Usamos la propiedad correcta UsuarioId
                UsuarioAnteriorId = equipo.UsuarioId,
                ZonaAnteriorId = equipo.ZonaId,
                UsuarioNuevoId = usuarioNuevoId,
                ZonaNuevaId = zonaNuevaId,
                Motivo = motivo,
                UsuarioResponsableId = usuarioResponsableId
            };

            // Actualizamos el equipo con los nuevos valores
            equipo.UsuarioId = usuarioNuevoId;
            equipo.ZonaId = zonaNuevaId;
            
            await _equipoRepo.ActualizarAsync(equipo, ct);
            await _repo.AgregarAsync(movimiento, ct);
        }
    }
}