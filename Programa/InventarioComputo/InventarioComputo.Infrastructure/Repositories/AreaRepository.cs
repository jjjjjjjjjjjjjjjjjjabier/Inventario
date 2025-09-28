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

        public async Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, bool incluirInactivas = false, CancellationToken ct = default)
        {
            var query = _context.Areas.Where(a => a.SedeId == sedeId);

            if (!incluirInactivas)
            {
                query = query.Where(a => a.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(a => a.Nombre.Contains(filtro));
            }

            return await query.OrderBy(a => a.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(int sedeId, string nombre, int? excluirId = null, CancellationToken ct = default)
        {
            var query = _context.Areas.Where(a => a.SedeId == sedeId && a.Nombre.ToLower() == nombre.ToLower());
            
            if (excluirId.HasValue)
            {
                query = query.Where(a => a.Id != excluirId.Value);
            }
            
            return await query.AnyAsync(ct);
        }

        public async Task<Area?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Areas.FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<Area> GuardarAsync(Area entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                await _context.Areas.AddAsync(entidad, ct);
            }
            else
            {
                var local = _context.Areas.Local.FirstOrDefault(e => e.Id == entidad.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Attach(entidad);
                _context.Entry(entidad).State = EntityState.Modified;
            }
            
            await _context.SaveChangesAsync(ct);
            return entidad;
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            var entidad = await _context.Areas.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}