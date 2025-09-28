using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IEquipoComputoRepository _equipoRepo;
        private readonly IHistorialMovimientoRepository _historialRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MovimientoService> _logger;

        public MovimientoService(
            IEquipoComputoRepository equipoRepo,
            IHistorialMovimientoRepository historialRepo,
            IUnitOfWork unitOfWork,
            ILogger<MovimientoService> logger)
        {
            _equipoRepo = equipoRepo;
            _historialRepo = historialRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<HistorialMovimiento> RegistrarMovimientoAsync(
            int equipoId,
            int? nuevoUsuarioId,
            int? nuevaZonaId,
            string motivo,
            int usuarioRegistraId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo es obligatorio", nameof(motivo));

            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}");

            var usuarioAnteriorId = equipo.UsuarioId;
            var zonaAnteriorId = equipo.ZonaId;

            var historial = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                UsuarioAnteriorId = usuarioAnteriorId,
                UsuarioNuevoId = nuevoUsuarioId,
                ZonaAnteriorId = zonaAnteriorId,
                ZonaNuevaId = nuevaZonaId,
                FechaMovimiento = DateTime.Now,
                Motivo = motivo,
                UsuarioResponsableId = usuarioRegistraId
            };

            equipo.UsuarioId = nuevoUsuarioId;
            equipo.ZonaId = nuevaZonaId;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await _equipoRepo.ActualizarAsync(equipo, ct);
                await _historialRepo.AgregarAsync(historial, ct);

                await transaction.CommitAsync(ct);

                return historial;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error al registrar movimiento para equipo {EquipoId}", equipoId);
                throw;
            }
        }

        public Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(
            int equipoId,
            CancellationToken ct = default)
        {
            return _historialRepo.ObtenerPorEquipoAsync(equipoId, ct);
        }
    }
}