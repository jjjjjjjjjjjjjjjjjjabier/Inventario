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
    public class EstadoRepository : IEstadoRepository
    {
        private readonly InventarioDbContext _context;

        public EstadoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Estado>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
        {
            var query = _context.Estados.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(e => e.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro) ||
                                        (e.Descripcion != null && e.Descripcion.Contains(filtro)));
            }

            return await query.OrderBy(e => e.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<Estado?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Estados.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Estado> GuardarAsync(Estado entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                await _context.Estados.AddAsync(entidad, ct);
            }
            else
            {
                var local = _context.Estados.Local.FirstOrDefault(e => e.Id == entidad.Id);
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
            var estado = await _context.Estados.FindAsync(new object[] { id }, ct);
            if (estado != null)
            {
                estado.Activo = false;
                _context.Entry(estado).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _context.Estados.AsQueryable();

            if (idExcluir.HasValue)
            {
                query = query.Where(e => e.Id != idExcluir.Value);
            }

            return query.AnyAsync(e => e.Nombre.ToLower() == nombre.ToLower(), ct);
        }
    }
}