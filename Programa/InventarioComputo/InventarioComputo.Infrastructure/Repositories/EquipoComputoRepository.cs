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
    public class EquipoComputoRepository : IEquipoComputoRepository
    {
        private readonly InventarioDbContext _context;

        public EquipoComputoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct)
        {
            await _context.EquiposComputo.AddAsync(equipo, ct);
            await _context.SaveChangesAsync(ct);
            return equipo;
        }

        public async Task EliminarAsync(int id, CancellationToken ct)
        {
            var equipo = await _context.EquiposComputo.FindAsync(new object[] { id }, ct);
            if (equipo != null)
            {
                // **CAMBIO CLAVE: Implementación de Soft Delete**
                equipo.Activo = false;
                _context.Entry(equipo).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? idExcluir, CancellationToken ct)
        {
            var query = _context.EquiposComputo
                                .Where(e => e.NumeroSerie.ToLower() == numeroSerie.ToLower());

            if (idExcluir.HasValue && idExcluir.Value > 0)
            {
                query = query.Where(e => e.Id != idExcluir.Value);
            }

            return await query.AnyAsync(ct);
        }

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct)
        {
            var query = _context.EquiposComputo
                .Include(e => e.TipoEquipo)
                .Include(e => e.Estado)
                .Include(e => e.Zona)
                .AsNoTracking();

            if (!incluirInactivos)
            {
                query = query.Where(e => e.Activo);
            }

            return await query.ToListAsync(ct);
        }

        public async Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            return await _context.EquiposComputo
                .Include(e => e.TipoEquipo)
                .Include(e => e.Estado)
                .Include(e => e.Zona)
                    .ThenInclude(z => z!.Area)
                        .ThenInclude(a => a!.Sede)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct)
        {
            _context.EquiposComputo.Update(equipo);
            await _context.SaveChangesAsync(ct);
        }
    }
}