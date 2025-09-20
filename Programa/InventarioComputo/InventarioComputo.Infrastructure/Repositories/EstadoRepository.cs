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

        public async Task<IReadOnlyList<Estado>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct)
        {
            var query = _context.Estados.AsQueryable();

            if (!incluirInactivas)
            {
                query = query.Where(e => e.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro) || (e.Descripcion != null && e.Descripcion.Contains(filtro)));
            }

            return await query.OrderBy(e => e.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken ct)
        {
            var query = _context.Estados.Where(e => e.Nombre.ToLower() == nombre.ToLower());

            if (excluirId.HasValue)
            {
                query = query.Where(e => e.Id != excluirId.Value);
            }

            return await query.AnyAsync(ct);
        }

        public async Task<Estado?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.Estados.FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<Estado> GuardarAsync(Estado entidad, CancellationToken ct)
        {
            if (entidad.Id == 0)
            {
                await _context.Estados.AddAsync(entidad, ct);
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
            var entidad = await _context.Estados.FindAsync(new object[] { id }, ct);
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