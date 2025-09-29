using System;
using System.Threading;
using System.Threading.Tasks;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;

namespace InventarioComputo.Infrastructure.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly InventarioDbContext _ctx;

        public BitacoraService(InventarioDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task RegistrarAsync(string entidad, string accion, int entidadId, int? usuarioResponsableId, string? detalles = null, CancellationToken ct = default)
        {
            var evt = new BitacoraEvento
            {
                Fecha = DateTime.Now,
                Entidad = entidad,
                Accion = accion,
                EntidadId = entidadId,
                UsuarioResponsableId = usuarioResponsableId,
                Detalles = detalles
            };

            await _ctx.BitacoraEventos.AddAsync(evt, ct);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}