using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Tests.Services
{
    [TestClass]
    public class MovimientoServiceTests
    {
        private Mock<IHistorialMovimientoRepository> _mockHistorialRepo;
        private Mock<IEquipoComputoRepository> _mockEquipoRepo;
        private Mock<IZonaRepository> _mockZonaRepo;
        private Mock<IAreaRepository> _mockAreaRepo;
        private Mock<ISedeRepository> _mockSedeRepo;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ISessionService> _mockSession;
        private MovimientoService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockHistorialRepo = new Mock<IHistorialMovimientoRepository>();
            _mockEquipoRepo = new Mock<IEquipoComputoRepository>();
            _mockZonaRepo = new Mock<IZonaRepository>();
            _mockAreaRepo = new Mock<IAreaRepository>();
            _mockSedeRepo = new Mock<ISedeRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSession = new Mock<ISessionService>();

            // Simulamos un usuario logueado
            _mockSession.Setup(s => s.UsuarioActual).Returns(new Usuario { Id = 1, NombreUsuario = "admin" });

            _service = new MovimientoService(
                _mockHistorialRepo.Object,
                _mockEquipoRepo.Object,
                _mockZonaRepo.Object,
                _mockAreaRepo.Object,
                _mockSedeRepo.Object,
                _mockUnitOfWork.Object,
                _mockSession.Object);
        }

        [TestMethod]
        public async Task AsignarEquipoAsync_ConDatosValidos_DebeAsignarCorrectamente()
        {
            // Arrange
            var equipoId = 1;
            var empleadoId = 10;
            var zonaId = 5;
            var motivo = "Asignación a nuevo empleado";

            var equipo = new EquipoComputo
            {
                Id = equipoId,
                NumeroSerie = "ABC123",
                EtiquetaInventario = "INV-001",
                TipoEquipoId = 1,
                EstadoId = 1,
                Activo = true
            };

            var zona = new Zona
            {
                Id = zonaId,
                AreaId = 3,
                Nombre = "Zona Test",
                Activo = true
            };

            var area = new Area
            {
                Id = 3,
                SedeId = 2,
                Nombre = "Area Test",
                Activo = true
            };

            // Mock de la transacción
            var mockTransaction = new Mock<IAppTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            // Mock del repositorio de equipo
            _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(equipoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipo);

            // Mock del repositorio de zona
            _mockZonaRepo.Setup(r => r.ObtenerPorIdAsync(zonaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(zona);

            // Mock del repositorio de área
            _mockAreaRepo.Setup(r => r.ObtenerPorIdAsync(zona.AreaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            // Act
            await _service.AsignarEquipoAsync(equipoId, empleadoId, zonaId, motivo);

            // Assert
            _mockHistorialRepo.Verify(r => r.AgregarAsync(
                It.IsAny<HistorialMovimiento>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockEquipoRepo.Verify(r => r.ActualizarAsync(
                It.Is<EquipoComputo>(e =>
                    e.Id == equipoId &&
                    e.EmpleadoId == empleadoId &&
                    e.ZonaId == zonaId),
                It.IsAny<CancellationToken>()), Times.Once);

            mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task AsignarEquipoAsync_SinMotivo_DebeLanzarExcepcion()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _service.AsignarEquipoAsync(1, 2, 3, ""));
        }
    }
}