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
    public class ZonaRepository : IZonaRepository
    {
        private readonly InventarioDbContext _context;

        public ZonaRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, bool incluirInactivas = false, CancellationToken ct = default)
        {
            var query = _context.Zonas.Where(z => z.AreaId == areaId);
            
            if (!incluirInactivas)
            {
                query = query.Where(z => z.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(z => z.Nombre.Contains(filtro));
            }

            return await query.OrderBy(z => z.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Zona>> ObtenerTodasAsync(bool incluirInactivas = false, CancellationToken ct = default)
        {
            var query = _context.Zonas.AsQueryable();
            if (!incluirInactivas)
            {
                query = query.Where(z => z.Activo);
            }
            return await query.OrderBy(z => z.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(int areaId, string nombre, int? excluirId = null, CancellationToken ct = default)
        {
            var query = _context.Zonas.Where(z => z.AreaId == areaId && z.Nombre.ToLower() == nombre.ToLower());
            
            if (excluirId.HasValue)
            {
                query = query.Where(z => z.Id != excluirId.Value);
            }
            
            return await query.AnyAsync(ct);
        }

        public async Task<Zona?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Zonas.FirstOrDefaultAsync(z => z.Id == id, ct);
        }

        public async Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                await _context.Zonas.AddAsync(entidad, ct);
            }
            else
            {
                var local = _context.Zonas.Local.FirstOrDefault(e => e.Id == entidad.Id);
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
            var entidad = await _context.Zonas.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}