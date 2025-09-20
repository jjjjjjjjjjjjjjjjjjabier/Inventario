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
        private readonly InventarioDbContext _ctx;

        public UnidadRepository(InventarioDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct)
        {
            var query = _ctx.Unidades.AsQueryable();

            if (!incluirInactivas)
            {
                query = query.Where(u => u.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var filtroLower = filtro.ToLower();
                query = query.Where(u =>
                    (u.Nombre != null && u.Nombre.ToLower().Contains(filtroLower)) ||
                    (u.Abreviatura != null && u.Abreviatura.ToLower().Contains(filtroLower))
                );
            }

            return await query.OrderBy(u => u.Nombre).AsNoTracking().ToListAsync(ct);
        }

        public Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return _ctx.Unidades.FindAsync(new object[] { id }, ct).AsTask();
        }

        public async Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct)
        {
            if (entidad.Id > 0)
            {
                _ctx.Entry(entidad).State = EntityState.Modified;
            }
            else
            {
                _ctx.Unidades.Add(entidad);
            }
            await _ctx.SaveChangesAsync(ct);
            return entidad;
        }

        public async Task EliminarAsync(int id, CancellationToken ct)
        {
            var entidad = await _ctx.Unidades.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                _ctx.Unidades.Remove(entidad);
                await _ctx.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir, CancellationToken ct)
        {
            var query = _ctx.Unidades.Where(u => u.Nombre.ToLower() == nombre.ToLower());
            if (idExcluir.HasValue)
            {
                query = query.Where(u => u.Id != idExcluir.Value);
            }
            return await query.AnyAsync(ct);
        }

        public async Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? idExcluir, CancellationToken ct)
        {
            var query = _ctx.Unidades.Where(u => u.Abreviatura.ToLower() == abreviatura.ToLower());
            if (idExcluir.HasValue)
            {
                query = query.Where(u => u.Id != idExcluir.Value);
            }
            return await query.AnyAsync(ct);
        }
    }
}