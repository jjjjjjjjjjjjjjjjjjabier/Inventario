using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace InventarioComputo.Infrastructure.Repositories
{
    public class ZonaRepository : IZonaRepository
    {
        private readonly InventarioDbContext _context;

        public ZonaRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, CancellationToken ct)
        {
            var query = _context.Zonas.Where(z => z.AreaId == areaId);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(z => z.Nombre.Contains(filtro));
            }

            return await query.AsNoTracking().ToListAsync(ct);
        }

        public async Task<bool> ExisteNombreAsync(int areaId, string nombre, int? excluirId, CancellationToken ct)
        {
            var query = _context.Zonas.Where(z => z.AreaId == areaId && z.Nombre.ToLower() == nombre.ToLower());
            if (excluirId.HasValue)
            {
                query = query.Where(z => z.Id != excluirId.Value);
            }
            return await query.AnyAsync(ct);
        }

        public async Task<Zona?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.Zonas.FirstOrDefaultAsync(z => z.Id == id, ct);
        }

        public async Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct)
        {
            if (entidad.Id == 0)
            {
                await _context.Zonas.AddAsync(entidad, ct);
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
            var entidad = await _context.Zonas.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                _context.Zonas.Remove(entidad);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}