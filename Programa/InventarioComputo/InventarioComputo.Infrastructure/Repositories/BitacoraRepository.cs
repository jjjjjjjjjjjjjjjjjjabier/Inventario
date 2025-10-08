using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Persistencia;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Infrastructure.Repositories
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly InventarioDbContext _ctx;

        public BitacoraRepository(InventarioDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BitacoraEvento> AgregarAsync(BitacoraEvento evento, CancellationToken ct = default)
        {
            await _ctx.BitacoraEventos.AddAsync(evento, ct);
            await _ctx.SaveChangesAsync(ct);
            return evento;
        }
    }
}