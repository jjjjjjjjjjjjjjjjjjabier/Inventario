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
    public class UnidadRepository : IUnidadRepository
    {
        private readonly InventarioDbContext _context;

        public UnidadRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default)
        {
            var query = _context.Unidades.AsQueryable();

            if (!incluirInactivas)
            {
                query = query.Where(u => u.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(u => u.Nombre.Contains(filtro) || u.Abreviatura.Contains(filtro));
            }

            return await query.OrderBy(u => u.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _context.Unidades.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                await _context.Unidades.AddAsync(entidad, ct);
            }
            else
            {
                var local = _context.Unidades.Local.FirstOrDefault(e => e.Id == entidad.Id);
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
            var entidad = await _context.Unidades.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _context.Unidades.Where(u => u.Nombre.ToLower() == nombre.ToLower());
            if (idExcluir.HasValue)
            {
                query = query.Where(u => u.Id != idExcluir.Value);
            }
            return query.AsNoTracking().AnyAsync(ct);
        }

        public Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _context.Unidades.Where(u => u.Abreviatura.ToLower() == abreviatura.ToLower());
            if (idExcluir.HasValue)
            {
                query = query.Where(u => u.Id != idExcluir.Value);
            }
            return query.AsNoTracking().AnyAsync(ct);
        }
    }
}