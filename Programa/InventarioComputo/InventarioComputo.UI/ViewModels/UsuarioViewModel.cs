using System.Collections.Generic;
using System.Linq;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.UI.ViewModels
{
    public class UsuarioViewModel
    {
        private readonly Usuario _usuario;
        private readonly IReadOnlyList<Rol> _roles;

        public UsuarioViewModel(Usuario usuario, IReadOnlyList<Rol> roles)
        {
            _usuario = usuario;
            _roles = roles;
        }

        public int Id => _usuario.Id;
        public string NombreUsuario => _usuario.NombreUsuario;
        public string NombreCompleto => _usuario.NombreCompleto;
        public bool Activo => _usuario.Activo;
        public bool EsEmpleadoSolamente => _usuario.EsEmpleadoSolamente;

        public string RolesTexto => string.Join(", ", _roles.Select(r => r.Nombre));

        public Usuario UsuarioOriginal => _usuario;
    }
}