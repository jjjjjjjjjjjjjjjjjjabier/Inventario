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
        private readonly IUnitOfWork _unitOfWork;

        public HistorialMovimientoService(
            IHistorialMovimientoRepository repo,
            IEquipoComputoRepository equipoRepo,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _equipoRepo = equipoRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(int equipoId, CancellationToken ct = default)
        {
            return await _repo.ObtenerHistorialEquipoAsync(equipoId, ct);
        }

        public async Task RegistrarMovimientoAsync(int equipoId, int? usuarioNuevoId, int? zonaNuevaId,
            string motivo, int usuarioResponsableId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo del movimiento es obligatorio.");
                
            // Obtener equipo actual para conocer su estado
            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct)
                ?? throw new InvalidOperationException($"No se encontró el equipo con ID {equipoId}");
                
            // Crear historial
            var historial = new HistorialMovimiento
            {
                EquipoComputoId = equipoId,
                UsuarioAnteriorId = equipo.UsuarioId,
                UsuarioNuevoId = usuarioNuevoId,
                ZonaAnteriorId = equipo.ZonaId,
                ZonaNuevaId = zonaNuevaId,
                FechaMovimiento = DateTime.Now,
                Motivo = motivo,
                UsuarioResponsableId = usuarioResponsableId
            };
            
            // Actualizar equipo
            equipo.UsuarioId = usuarioNuevoId;
            equipo.ZonaId = zonaNuevaId;
            
            // Guardar en una transacción
            await _repo.AgregarAsync(historial, ct);
            await _equipoRepo.ActualizarAsync(equipo, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}