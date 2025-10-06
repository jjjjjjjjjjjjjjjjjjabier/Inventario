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
        private readonly IHistorialMovimientoRepository _historialRepo;
        private readonly IEquipoComputoRepository _equipoRepo;
        private readonly IZonaRepository _zonaRepo;
        private readonly IAreaRepository _areaRepo;
        private readonly ISedeRepository _sedeRepo;
        private readonly IUnitOfWork _uow;
        private readonly ISessionService _session;

        public MovimientoService(
            IHistorialMovimientoRepository historialRepo,
            IEquipoComputoRepository equipoRepo,
            IZonaRepository zonaRepo,
            IAreaRepository areaRepo,
            ISedeRepository sedeRepo,
            IUnitOfWork uow,
            ISessionService session)
        {
            _historialRepo = historialRepo;
            _equipoRepo = equipoRepo;
            _zonaRepo = zonaRepo;
            _areaRepo = areaRepo;
            _sedeRepo = sedeRepo;
            _uow = uow;
            _session = session;
        }

        public Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(int equipoId, CancellationToken ct = default)
            => _historialRepo.ObtenerPorEquipoAsync(equipoId, ct);

        public async Task AsignarEquipoAsync(int equipoId, int? empleadoId, int? zonaId, string motivo, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));

            var usuarioRespId = _session.UsuarioActual?.Id;
            if (usuarioRespId == null || usuarioRespId == 0)
                throw new InvalidOperationException("No se pudo determinar el usuario en sesión. Por favor, cierre sesión y vuelva a iniciarla.");

            var actual = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}.");

            // Valores anteriores
            int? empleadoAnteriorId = actual.EmpleadoId;
            int? zonaAnteriorId = actual.ZonaId;
            int? areaAnteriorId = actual.AreaId ?? actual.Zona?.AreaId;
            int? sedeAnteriorId = actual.SedeId ?? actual.Zona?.Area?.SedeId;

            // Mantener ubicación si no se recibe nueva zona
            int? nuevaZonaId = zonaId ?? actual.ZonaId;
            int? areaNuevaId = actual.AreaId ?? actual.Zona?.AreaId;
            int? sedeNuevaId = actual.SedeId ?? actual.Zona?.Area?.SedeId;

            if (zonaId.HasValue)
            {
                var zona = await _zonaRepo.ObtenerPorIdAsync(zonaId.Value, ct)
                    ?? throw new InvalidOperationException("La zona seleccionada no existe.");
                areaNuevaId = zona.AreaId;

                var area = await _areaRepo.ObtenerPorIdAsync(zona.AreaId, ct)
                    ?? throw new InvalidOperationException("El área asociada a la zona no existe.");
                sedeNuevaId = area.SedeId;
            }

            var historial = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                FechaMovimiento = DateTime.Now,
                Motivo = motivo.Trim(),
                UsuarioResponsableId = usuarioRespId.Value,
                EmpleadoAnteriorId = empleadoAnteriorId,
                EmpleadoNuevoId = empleadoId,
                ZonaAnteriorId = zonaAnteriorId,
                ZonaNuevaId = nuevaZonaId,
                AreaAnteriorId = areaAnteriorId,
                AreaNuevaId = areaNuevaId,
                SedeAnteriorId = sedeAnteriorId,
                SedeNuevaId = sedeNuevaId
            };

            var equipoActualizar = new EquipoComputo
            {
                Id = equipoId,
                EmpleadoId = empleadoId,
                ZonaId = nuevaZonaId,
                AreaId = areaNuevaId,
                SedeId = sedeNuevaId,

                // Mantener datos requeridos
                TipoEquipoId = actual.TipoEquipoId,
                EstadoId = actual.EstadoId,
                NumeroSerie = actual.NumeroSerie,
                EtiquetaInventario = actual.EtiquetaInventario,
                Marca = actual.Marca,
                Modelo = actual.Modelo,
                Caracteristicas = actual.Caracteristicas,
                Observaciones = actual.Observaciones,
                FechaAdquisicion = actual.FechaAdquisicion,
                Costo = actual.Costo,
                Activo = actual.Activo
            };

            await using var tx = await _uow.BeginTransactionAsync(ct);
            try
            {
                await _historialRepo.AgregarAsync(historial, ct);
                await _equipoRepo.ActualizarAsync(equipoActualizar, ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RegistrarMovimientoAsync(int equipoId, int? empleadoNuevoId, int? zonaNuevaId, string motivo, int usuarioResponsableId, CancellationToken ct = default)
        {
            await AsignarEquipoAsync(equipoId, empleadoNuevoId, zonaNuevaId, motivo, ct);
        }
    }
}