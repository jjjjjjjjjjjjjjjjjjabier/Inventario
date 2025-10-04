using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IEquipoComputoRepository _equipoRepo;
        private readonly IHistorialMovimientoRepository _histRepo;
        private readonly ISessionService _session;

        public MovimientoService(IEquipoComputoRepository equipoRepo,
                                 IHistorialMovimientoRepository histRepo,
                                 ISessionService session)
        {
            _equipoRepo = equipoRepo;
            _histRepo = histRepo;
            _session = session;
        }

        public Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(int equipoId, CancellationToken ct = default)
            => _histRepo.ObtenerHistorialEquipoAsync(equipoId, ct);

        public async Task AsignarEquipoAsync(int equipoId, int? empleadoId, int? zonaId, string motivo, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));

            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct) ?? throw new InvalidOperationException("Equipo no encontrado.");

            var anteriorEmpleadoId = equipo.EmpleadoId;
            var anteriorZonaId = equipo.ZonaId;
            var anteriorAreaId = equipo.AreaId;
            var anteriorSedeId = equipo.SedeId;

            // Actualizar asignación actual
            equipo.EmpleadoId = empleadoId;
            equipo.ZonaId = zonaId;

            // Si zona cambia, derivar area/sede (el repo del equipo debe manejarlos en capa infra si corresponde)
            await _equipoRepo.ActualizarAsync(equipo, ct);

            // Registrar historial
            var hist = new HistorialMovimiento
            {
                EquipoComputoId = equipo.Id,
                FechaMovimiento = DateTime.UtcNow,
                EmpleadoAnteriorId = anteriorEmpleadoId,
                ZonaAnteriorId = anteriorZonaId,
                AreaAnteriorId = anteriorAreaId,
                SedeAnteriorId = anteriorSedeId,
                EmpleadoNuevoId = empleadoId,
                ZonaNuevaId = zonaId,
                AreaNuevaId = equipo.AreaId,
                SedeNuevaId = equipo.SedeId,
                Motivo = motivo.Trim(),
                UsuarioResponsableId = _session.UsuarioActual?.Id ?? 0
            };

            await _histRepo.AgregarAsync(hist, ct);
        }

        public Task RegistrarMovimientoAsync(int equipoId, int? empleadoNuevoId, int? zonaNuevaId, string motivo, int usuarioResponsableId, CancellationToken ct = default)
            => AsignarEquipoAsync(equipoId, empleadoNuevoId, zonaNuevaId, motivo, ct);
    }
}