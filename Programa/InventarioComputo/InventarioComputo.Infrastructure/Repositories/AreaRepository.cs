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
    public class AreaRepository : IAreaRepository
    {
        private readonly InventarioDbContext _context;

        public AreaRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, CancellationToken ct)
        {
            var query = _context.Areas
                .Where(a => a.SedeId == sedeId && a.Activo); // **CAMBIO:** Por defecto solo activas

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(a => a.Nombre.Contains(filtro));
            }

            return await query.OrderBy(a => a.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(int sedeId, string nombre, int? excluirId, CancellationToken ct)
        {
            var query = _context.Areas.Where(a => a.SedeId == sedeId && a.Nombre.ToLower() == nombre.ToLower());
            if (excluirId.HasValue)
            {
                query = query.Where(a => a.Id != excluirId.Value);
            }
            return await query.AnyAsync(ct);
        }

        public async Task<Area?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.Areas.FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<Area> GuardarAsync(Area entidad, CancellationToken ct)
        {
            if (entidad.Id == 0)
            {
                await _context.Areas.AddAsync(entidad, ct);
            }
            else
            {
                _context.Entry(entidad).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync(ct);
            return entidad;
        }

        public async Task EliminarAsync(int id, CancellationToken ct)
        {
            var entidad = await _context.Areas.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                // **CAMBIO CLAVE: Implementación de Soft Delete**
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}