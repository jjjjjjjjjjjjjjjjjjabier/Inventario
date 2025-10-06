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
    public class TipoEquipoRepository : ITipoEquipoRepository
    {
        private readonly InventarioDbContext _context;

        public TipoEquipoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TipoEquipo>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
        {
            var query = _context.TiposEquipo.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(t => t.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(t => t.Nombre.Contains(filtro));
            }

            return await query.OrderBy(t => t.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public Task<TipoEquipo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _context.TiposEquipo.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<TipoEquipo> GuardarAsync(TipoEquipo entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                // Validar que no haya otro TipoEquipo con el mismo nombre (case-insensitive)
                var existe = await _context.TiposEquipo
                    .AnyAsync(t => t.Nombre.ToLower() == entidad.Nombre.ToLower(), ct);

                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe un tipo de equipo con el nombre '{entidad.Nombre}'.");
                }

                await _context.TiposEquipo.AddAsync(entidad, ct);
            }
            else
            {
                // Validar que no haya otro TipoEquipo con el mismo nombre (case-insensitive)
                var existe = await _context.TiposEquipo
                    .Where(t => t.Id != entidad.Id)
                    .AnyAsync(t => t.Nombre.ToLower() == entidad.Nombre.ToLower(), ct);

                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe un tipo de equipo con el nombre '{entidad.Nombre}'.");
                }

                var local = _context.TiposEquipo.Local.FirstOrDefault(t => t.Id == entidad.Id);
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
            var tipoEquipo = await _context.TiposEquipo.FindAsync(new object[] { id }, ct);
            if (tipoEquipo != null)
            {
                tipoEquipo.Activo = false;
                _context.Entry(tipoEquipo).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _context.TiposEquipo.AsQueryable();
            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.Id != idExcluir.Value);
            }
            return query.AnyAsync(t => t.Nombre.ToLower() == nombre.ToLower(), ct);
        }
    }
}