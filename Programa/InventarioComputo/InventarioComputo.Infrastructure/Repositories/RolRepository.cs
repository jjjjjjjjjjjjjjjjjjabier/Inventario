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

        public async Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == nombre, ct);
        }

        public async Task<int> CrearRolAsync(Rol rol, CancellationToken ct = default)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync(ct);
            return rol.Id;
        }

        public async Task AsignarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            // Verificar si ya existe la relación
            var existe = await _context.UsuarioRoles
                .AnyAsync(ur => ur.UsuarioId == usuarioId && ur.RolId == rolId, ct);

            if (!existe)
            {
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuarioId, RolId = rolId });
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IReadOnlyList<Rol>> ObtenerRolesPorUsuarioIdAsync(int usuarioId, CancellationToken ct = default)
        {
            return await _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuarioId)
                .Include(ur => ur.Rol)
                .Select(ur => ur.Rol)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken ct = default)
        {
            return await _context.Roles.OrderBy(r => r.Nombre).ToListAsync(ct);
        }

        public Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}