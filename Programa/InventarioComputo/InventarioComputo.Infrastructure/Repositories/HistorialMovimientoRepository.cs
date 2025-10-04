using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Infrastructure.Repositories
{
    public class HistorialMovimientoRepository : IHistorialMovimientoRepository
    {
        private readonly InventarioDbContext _context;

        public HistorialMovimientoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<HistorialMovimiento> AgregarAsync(HistorialMovimiento entidad, CancellationToken ct = default)
        {
            await _context.HistorialMovimientos.AddAsync(entidad, ct);
            await _context.SaveChangesAsync(ct);
            return entidad;
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(int equipoId, CancellationToken ct = default)
        {
            return await _context.HistorialMovimientos
                .Include(h => h.EmpleadoAnterior)
                .Include(h => h.EmpleadoNuevo)
                .Include(h => h.UsuarioResponsable)
                .Include(h => h.ZonaAnterior).ThenInclude(z => z.Area).ThenInclude(a => a.Sede)
                .Include(h => h.ZonaNueva).ThenInclude(z => z.Area).ThenInclude(a => a.Sede)
                .Where(h => h.EquipoComputoId == equipoId)
                .OrderByDescending(h => h.FechaMovimiento)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorEquipoAsync(int equipoId, CancellationToken ct = default)
        {
            return await _context.HistorialMovimientos
                .Where(h => h.EquipoComputoId == equipoId)
                .Include(h => h.EmpleadoAnterior)
                .Include(h => h.EmpleadoNuevo)
                .Include(h => h.ZonaAnterior)
                .Include(h => h.ZonaNueva)
                .Include(h => h.UsuarioResponsable)
                .OrderByDescending(h => h.FechaMovimiento)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken ct = default)
        {
            // Nota: usuarioId se refiere al UsuarioResponsable (cuenta del sistema)
            return await _context.HistorialMovimientos
                .Where(h => h.UsuarioResponsableId == usuarioId)
                .Include(h => h.EquipoComputo)
                .Include(h => h.UsuarioResponsable)
                .OrderByDescending(h => h.FechaMovimiento)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}