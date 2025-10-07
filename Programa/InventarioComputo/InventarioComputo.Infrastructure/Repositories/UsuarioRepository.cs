using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly InventarioDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioRepository(InventarioDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            var usuario = await ObtenerPorNombreUsuarioAsync(username, ct);
            return usuario != null && _passwordHasher.VerifyPassword(password, usuario.PasswordHash);
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario, ct);
        }

        public async Task<int> CrearUsuarioAsync(Usuario usuario, CancellationToken ct = default)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(ct);
            return usuario.Id;
        }

        public async Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default)
        {
            return await _context.UsuarioRoles
                .Where(ur => ur.RolId == rolId)
                .Include(ur => ur.Usuario)
                .Select(ur => ur.Usuario)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Usuario>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(u => u.Activo);
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(u => u.NombreUsuario.Contains(filtro) ||
                                        u.NombreCompleto.Contains(filtro));
            }

            return (await query.ToListAsync(ct)).Where(u => u != null).Cast<Usuario>().ToList();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<Usuario> GuardarAsync(Usuario usuario, CancellationToken ct = default)
        {
            if (usuario.Id == 0)
            {
                _context.Usuarios.Add(usuario);
            }
            else
            {
                var local = _context.Usuarios.Local.FirstOrDefault(u => u.Id == usuario.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Attach(usuario);
                _context.Entry(usuario).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(ct);
            return usuario;
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            var usuario = await _context.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario != null)
            {
                usuario.Activo = false;
                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _context.Usuarios.Where(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());

            if (idExcluir.HasValue)
            {
                query = query.Where(u => u.Id != idExcluir.Value);
            }

            return await query.AnyAsync(ct);
        }

        public async Task<bool> VerificarCredencialesAsync(string nombreUsuario, string password, CancellationToken ct = default)
        {
            var usuario = await ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
            return usuario != null && _passwordHasher.VerifyPassword(password, usuario.PasswordHash);
        }

        public async Task AsignarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
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

        public async Task QuitarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            var relacion = await _context.UsuarioRoles
                .FirstOrDefaultAsync(ur => ur.UsuarioId == usuarioId && ur.RolId == rolId, ct);

            if (relacion != null)
            {
                _context.UsuarioRoles.Remove(relacion);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
        {
            return await _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuarioId)
                .Include(ur => ur.Rol)
                .Select(ur => ur.Rol)
                .ToListAsync(ct);
        }

        public Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, bool incluirInactivos, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}