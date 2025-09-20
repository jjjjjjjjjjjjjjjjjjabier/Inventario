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

        public async Task<IReadOnlyList<TipoEquipo>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct)
        {
            var query = _context.TiposEquipo.AsQueryable();

            if (!incluirInactivas)
            {
                query = query.Where(t => t.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(t => t.Nombre.Contains(filtro));
            }

            return await query.OrderBy(t => t.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<TipoEquipo?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.TiposEquipo.FindAsync(new object[] { id }, ct);
        }

        public async Task<TipoEquipo> GuardarAsync(TipoEquipo entidad, CancellationToken ct)
        {
            if (entidad.Id == 0)
            {
                await _context.TiposEquipo.AddAsync(entidad, ct);
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
            var entidad = await _context.TiposEquipo.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                // **CAMBIO CLAVE: Implementación de Soft Delete**
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir, CancellationToken ct)
        {
            var query = _context.TiposEquipo.Where(t => t.Nombre.ToLower() == nombre.ToLower());

            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.Id != idExcluir.Value);
            }

            return await query.AnyAsync(ct);
        }
    }
}