using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface ISessionService
    {
        event EventHandler<bool>? SesionCambiada;
        Usuario? UsuarioActual { get; }
        bool EstaAutenticado { get; }
        Task<bool> IniciarSesionAsync(string usuario, string password);
        void CerrarSesion();
        bool TieneRol(string rolNombre);
        IReadOnlyList<Rol> ObtenerRolesUsuario();
    }
}