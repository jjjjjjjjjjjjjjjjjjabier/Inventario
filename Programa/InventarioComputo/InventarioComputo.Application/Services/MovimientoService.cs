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
        private readonly IBitacoraService _bitacora;
        private readonly ISessionService _session;
        private readonly IHistorialMovimientoRepository _historialRepo;

        public MovimientoService(
            IEquipoComputoRepository equipoRepo,
            IBitacoraService bitacora,
            ISessionService session,
            IHistorialMovimientoRepository historialRepo)
        {
            _equipoRepo = equipoRepo;
            _bitacora = bitacora;
            _session = session;
            _historialRepo = historialRepo;
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(int equipoId, CancellationToken ct = default)
        {
            return await _historialRepo.ObtenerHistorialEquipoAsync(equipoId, ct);
        }

        public async Task AsignarEquipoAsync(int equipoId, int? usuarioId, int? zonaId, string motivo, CancellationToken ct = default)
        {
            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}");

            // Guardamos los valores anteriores
            var usuarioAnteriorId = equipo.UsuarioId;
            var zonaAnteriorId = equipo.ZonaId;

            // Actualizamos con los nuevos valores
            equipo.UsuarioId = usuarioId;
            equipo.ZonaId = zonaId;

            await _equipoRepo.ActualizarAsync(equipo, ct);

            // Registramos el movimiento en la bitácora
            await _bitacora.RegistrarAsync(
                entidad: "Movimiento",
                accion: "AsignacionEquipo",
                entidadId: equipoId,
                usuarioResponsableId: _session.UsuarioActual?.Id,
                detalles: $"De: Usuario={usuarioAnteriorId}, Zona={zonaAnteriorId} A: Usuario={usuarioId}, Zona={zonaId}. Motivo: {motivo}",
                ct: ct);
        }

        public async Task RegistrarMovimientoAsync(int equipoId, int? usuarioNuevoId, int? zonaNuevaId, string motivo, int usuarioResponsableId, CancellationToken ct = default)
        {
            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}");

            var movimiento = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                FechaMovimiento = DateTime.Now,
                UsuarioAnteriorId = equipo.UsuarioId,
                ZonaAnteriorId = equipo.ZonaId,
                UsuarioNuevoId = usuarioNuevoId,
                ZonaNuevaId = zonaNuevaId,
                Motivo = motivo,
                UsuarioResponsableId = usuarioResponsableId
            };

            // Actualizamos el equipo
            equipo.UsuarioId = usuarioNuevoId;
            equipo.ZonaId = zonaNuevaId;

            await _equipoRepo.ActualizarAsync(equipo, ct);
            await _historialRepo.AgregarAsync(movimiento, ct);
        }
    }
}