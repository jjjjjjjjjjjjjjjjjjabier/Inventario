using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class EquipoComputoService : IEquipoComputoService
    {
        private readonly IEquipoComputoRepository _repo;
        private readonly IBitacoraService _bitacora;
        private readonly ISessionService _session;

        public EquipoComputoService(IEquipoComputoRepository repo, IBitacoraService bitacora, ISessionService session)
        {
            _repo = repo;
            _bitacora = bitacora;
            _session = session;
        }

        public Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default)
            => _repo.ObtenerTodosAsync(incluirInactivos, ct);

        public Task<IReadOnlyList<EquipoComputo>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
            => _repo.BuscarAsync(filtro, incluirInactivos, ct);

        public Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public async Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            ValidarEquipo(equipo);

            if (await _repo.ExistsByNumeroSerieAsync(equipo.NumeroSerie, null, ct))
                throw new InvalidOperationException($"Ya existe un equipo con el número de serie '{equipo.NumeroSerie}'.");

            if (await _repo.ExistsByEtiquetaInventarioAsync(equipo.EtiquetaInventario, null, ct))
                throw new InvalidOperationException($"Ya existe un equipo con la etiqueta '{equipo.EtiquetaInventario}'.");

            var agregado = await _repo.AgregarAsync(equipo, ct);

            await _bitacora.RegistrarAsync(
                entidad: "EquipoComputo",
                accion: "Alta",
                entidadId: agregado.Id,
                usuarioResponsableId: _session.UsuarioActual?.Id,
                detalles: $"Serie={agregado.NumeroSerie}; Etiqueta={agregado.EtiquetaInventario}",
                ct: ct);

            return agregado;
        }

        public async Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default)
        {
            ValidarEquipo(equipo);

            if (await _repo.ExistsByNumeroSerieAsync(equipo.NumeroSerie, equipo.Id, ct))
                throw new InvalidOperationException($"Ya existe otro equipo con el número de serie '{equipo.NumeroSerie}'.");

            if (await _repo.ExistsByEtiquetaInventarioAsync(equipo.EtiquetaInventario, equipo.Id, ct))
                throw new InvalidOperationException($"Ya existe otro equipo con la etiqueta '{equipo.EtiquetaInventario}'.");

            await _repo.ActualizarAsync(equipo, ct);

            await _bitacora.RegistrarAsync(
                entidad: "EquipoComputo",
                accion: "Actualizacion",
                entidadId: equipo.Id,
                usuarioResponsableId: _session.UsuarioActual?.Id,
                detalles: $"Serie={equipo.NumeroSerie}; Etiqueta={equipo.EtiquetaInventario}",
                ct: ct);
        }

        public async Task EliminarAsync(int id, CancellationToken ct = default)
        {
            await _repo.EliminarAsync(id, ct);

            await _bitacora.RegistrarAsync(
                entidad: "EquipoComputo",
                accion: "BajaLogica",
                entidadId: id,
                usuarioResponsableId: _session.UsuarioActual?.Id,
                detalles: null,
                ct: ct);
        }

        private static void ValidarEquipo(EquipoComputo e)
        {
            if (e is null) throw new ArgumentNullException(nameof(e));

            e.NumeroSerie = e.NumeroSerie?.Trim() ?? string.Empty;
            e.EtiquetaInventario = e.EtiquetaInventario?.Trim() ?? string.Empty;
            e.Marca = e.Marca?.Trim() ?? string.Empty;
            e.Modelo = e.Modelo?.Trim() ?? string.Empty;
            e.Caracteristicas = e.Caracteristicas?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(e.NumeroSerie)) throw new ArgumentException("El número de serie es obligatorio.", nameof(e.NumeroSerie));
            if (string.IsNullOrWhiteSpace(e.EtiquetaInventario)) throw new ArgumentException("La etiqueta de inventario es obligatoria.", nameof(e.EtiquetaInventario));
            if (string.IsNullOrWhiteSpace(e.Marca)) throw new ArgumentException("La marca es obligatoria.", nameof(e.Marca));
            if (string.IsNullOrWhiteSpace(e.Modelo)) throw new ArgumentException("El modelo es obligatorio.", nameof(e.Modelo));
            if (e.TipoEquipoId <= 0) throw new ArgumentException("El tipo de equipo es obligatorio.", nameof(e.TipoEquipoId));
            if (e.EstadoId <= 0) throw new ArgumentException("El estado es obligatorio.", nameof(e.EstadoId));

            if (e.NumeroSerie.Length > 100) throw new ArgumentException("Número de serie demasiado largo (máx. 100).", nameof(e.NumeroSerie));
            if (e.EtiquetaInventario.Length > 100) throw new ArgumentException("Etiqueta de inventario demasiado larga (máx. 100).", nameof(e.EtiquetaInventario));
            if (e.Marca.Length > 100) throw new ArgumentException("Marca demasiado larga (máx. 100).", nameof(e.Marca));
            if (e.Modelo.Length > 100) throw new ArgumentException("Modelo demasiado largo (máx. 100).", nameof(e.Modelo));
            if (e.Caracteristicas.Length > 500) throw new ArgumentException("Caracteristicas demasiado largas (máx. 500).", nameof(e.Caracteristicas));
            if (e.Observaciones?.Length > 1000) throw new ArgumentException("Observaciones demasiado largas (máx. 1000).", nameof(e.Observaciones));
        }
    }
}