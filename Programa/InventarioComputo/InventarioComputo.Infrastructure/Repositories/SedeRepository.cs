using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace InventarioComputo.Infrastructure.Repositories
{
    public class SedeRepository : ISedeRepository
    {
        private readonly InventarioDbContext _context;

        public SedeRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Sede>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct)
        {
            var query = _context.Sedes.AsQueryable();

            if (!incluirInactivas)
            {
                query = query.Where(s => s.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(s => s.Nombre.Contains(filtro));
            }

            return await query.AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken ct)
        {
            var query = _context.Sedes.Where(s => s.Nombre.ToLower() == nombre.ToLower());
            if (excluirId.HasValue)
            {
                query = query.Where(s => s.Id != excluirId.Value);
            }
            return await query.AnyAsync(ct);
        }

        public async Task<Sede?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.Sedes.FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<Sede> GuardarAsync(Sede entidad, CancellationToken ct)
        {
            if (entidad.Id == 0)
            {
                await _context.Sedes.AddAsync(entidad, ct);
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
            var entidad = await _context.Sedes.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                _context.Sedes.Remove(entidad);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}