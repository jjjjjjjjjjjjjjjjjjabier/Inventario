using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
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
        private readonly InventarioDbContext _ctx;

        public EquipoComputoRepository(InventarioDbContext ctx)
        {
            _ctx = ctx;
        }

        private IQueryable<EquipoComputo> BaseQuery() =>
            _ctx.EquiposComputo
                .Include(e => e.TipoEquipo)
                .Include(e => e.Estado)
                .Include(e => e.Usuario)
                .Include(e => e.Empleado) // NUEVO include
                .Include(e => e.Zona)
                    .ThenInclude(z => z.Area)
                        .ThenInclude(a => a.Sede);

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default)
        {
            var query = BaseQuery();
            if (!incluirInactivos)
                query = query.Where(e => e.Activo);
            return await query.AsNoTracking().ToListAsync(ct);
        }

        public async Task<IReadOnlyList<EquipoComputo>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
        {
            var query = BaseQuery();
            if (!incluirInactivos)
                query = query.Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.Trim();
                query = query.Where(e =>
                    e.NumeroSerie.Contains(f) ||
                    e.EtiquetaInventario.Contains(f) ||
                    e.Marca.Contains(f) ||
                    e.Modelo.Contains(f) ||
                    e.TipoEquipo.Nombre.Contains(f) ||
                    e.Estado.Nombre.Contains(f) ||
                    (e.Usuario != null && e.Usuario.NombreCompleto.Contains(f)) ||
                    (e.Empleado != null && e.Empleado.NombreCompleto.Contains(f)) // NUEVO filtro por empleado
                );
            }

            return await query.AsNoTracking().ToListAsync(ct);
        }

        public async Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await BaseQuery().FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            DesacoplarEntidadesRelacionadas(equipo);
            _ctx.EquiposComputo.Add(equipo);
            await _ctx.SaveChangesAsync(ct);
            return equipo;
        }

        public async Task<EquipoComputo> ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            DesacoplarEntidadesRelacionadas(equipo);

            var local = _ctx.EquiposComputo.Local.FirstOrDefault(e => e.Id == equipo.Id);
            if (local != null)
                _ctx.Entry(local).State = EntityState.Detached;

            _ctx.Attach(equipo);
            var entry = _ctx.Entry(equipo);
            entry.State = EntityState.Modified;

            entry.Property(e => e.TipoEquipoId).IsModified = true;
            entry.Property(e => e.EstadoId).IsModified = true;
            entry.Property(e => e.ZonaId).IsModified = true;
            entry.Property(e => e.EmpleadoId).IsModified = true; // NUEVO

            await _ctx.SaveChangesAsync(ct);
            return equipo;
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            var equipo = await _ctx.EquiposComputo.FindAsync(new object[] { id }, ct);
            if (equipo != null)
            {
                equipo.Activo = false;
                _ctx.Entry(equipo).State = EntityState.Modified;
                await _ctx.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _ctx.EquiposComputo.AsQueryable();
            if (idExcluir.HasValue)
                query = query.Where(e => e.Id != idExcluir.Value);
            return await query.AnyAsync(e => e.NumeroSerie == numeroSerie, ct);
        }

        public async Task<bool> ExistsByEtiquetaInventarioAsync(string etiqueta, int? idExcluir = null, CancellationToken ct = default)
        {
            var query = _ctx.EquiposComputo.AsQueryable();
            if (idExcluir.HasValue)
                query = query.Where(e => e.Id != idExcluir.Value);
            return await query.AnyAsync(e => e.EtiquetaInventario == etiqueta, ct);
        }

        private void DesacoplarEntidadesRelacionadas(EquipoComputo equipo)
        {
            equipo.TipoEquipo = null;
            equipo.Estado = null;
            equipo.Zona = null;
            equipo.Usuario = null;
            equipo.Empleado = null; // NUEVO
        }

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerParaReporteAsync(FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            var query = BaseQuery();

            if (!filtro.IncluirInactivos)
                query = query.Where(e => e.Activo);

            if (filtro.SedeId.HasValue)
                query = query.Where(e => e.Zona != null && e.Zona.Area != null && e.Zona.Area.SedeId == filtro.SedeId);

            if (filtro.AreaId.HasValue)
                query = query.Where(e => e.Zona != null && e.Zona.AreaId == filtro.AreaId);

            if (filtro.ZonaId.HasValue)
                query = query.Where(e => e.ZonaId == filtro.ZonaId);

            if (filtro.EstadoId.HasValue)
                query = query.Where(e => e.EstadoId == filtro.EstadoId);

            if (filtro.TipoEquipoId.HasValue)
                query = query.Where(e => e.TipoEquipoId == filtro.TipoEquipoId);

            if (filtro.UsuarioId.HasValue)
                query = query.Where(e => e.UsuarioId == filtro.UsuarioId);

            if (filtro.FechaDesde.HasValue)
            {
                var desde = filtro.FechaDesde.Value.Date;
                query = query.Where(e => e.FechaAdquisicion.HasValue && e.FechaAdquisicion.Value >= desde);
            }
            if (filtro.FechaHasta.HasValue)
            {
                var hastaExclusivo = filtro.FechaHasta.Value.Date.AddDays(1);
                query = query.Where(e => e.FechaAdquisicion.HasValue && e.FechaAdquisicion.Value < hastaExclusivo);
            }

            return await query.AsNoTracking().ToListAsync(ct);
        }
    }
}