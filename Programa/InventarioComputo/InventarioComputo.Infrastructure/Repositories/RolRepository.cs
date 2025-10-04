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
    public class RolRepository : IRolRepository
    {
        private readonly InventarioDbContext _context;

        public RolRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken ct = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Nombre)
                .ToListAsync(ct);
        }

        public async Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Nombre == nombre, ct);
        }

        public async Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default)
        {
            if (rol.Id == 0)
            {
                await _context.Roles.AddAsync(rol, ct);
            }
            else
            {
                var local = _context.Roles.Local.FirstOrDefault(e => e.Id == rol.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Attach(rol);
                _context.Entry(rol).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(ct);
            return rol;
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            // Eliminar relaciones de UsuarioRol para evitar conflictos de FK
            var relaciones = _context.UsuarioRoles.Where(ur => ur.RolId == id);
            _context.UsuarioRoles.RemoveRange(relaciones);

            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (rol != null)
            {
                _context.Roles.Remove(rol);
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Rol>> ObtenerRolesPorUsuarioIdAsync(int usuarioId, CancellationToken ct = default)
        {
            return await _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuarioId)
                .Include(ur => ur.Rol)
                .Select(ur => ur.Rol!)
                .AsNoTracking()
                .Distinct()
                .ToListAsync(ct);
        }

        public async Task AsignarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            var existe = await _context.UsuarioRoles
                .AnyAsync(ur => ur.UsuarioId == usuarioId && ur.RolId == rolId, ct);

            if (!existe)
            {
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuarioId, RolId = rolId });
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task QuitarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            // Clave compuesta configurada en OnModelCreating (UsuarioId, RolId)
            var relacion = await _context.UsuarioRoles
                .FirstOrDefaultAsync(ur => ur.UsuarioId == usuarioId && ur.RolId == rolId, ct);

            if (relacion != null)
            {
                _context.UsuarioRoles.Remove(relacion);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}