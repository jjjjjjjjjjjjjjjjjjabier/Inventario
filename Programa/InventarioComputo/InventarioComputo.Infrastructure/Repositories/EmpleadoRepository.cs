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
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly InventarioDbContext _context;

        public EmpleadoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Empleado>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
        {
            var query = _context.Empleados.AsQueryable();

            if (!incluirInactivos)
                query = query.Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Trim();
                query = query.Where(e =>
                    e.NombreCompleto.Contains(f) ||
                    (e.Puesto != null && e.Puesto.Contains(f)));
            }

            return await query.OrderBy(e => e.NombreCompleto).AsNoTracking().ToListAsync(ct);
        }

        public Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _context.Empleados.FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task<Empleado> GuardarAsync(Empleado entidad, CancellationToken ct = default)
        {
            if (entidad.Id == 0)
            {
                await _context.Empleados.AddAsync(entidad, ct);
            }
            else
            {
                var local = _context.Empleados.Local.FirstOrDefault(e => e.Id == entidad.Id);
                if (local != null)
                    _context.Entry(local).State = EntityState.Detached;

                _context.Attach(entidad);
                _context.Entry(entidad).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(ct);
            return entidad;
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            var entidad = await _context.Empleados.FindAsync(new object[] { id }, ct);
            if (entidad != null)
            {
                entidad.Activo = false;
                _context.Entry(entidad).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}