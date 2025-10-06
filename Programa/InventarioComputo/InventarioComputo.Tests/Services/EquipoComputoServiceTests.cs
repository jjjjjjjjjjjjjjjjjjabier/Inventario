using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Tests.Services
{
    [TestClass]
    public class EquipoComputoServiceTests
    {
        private Mock<IEquipoComputoRepository> _mockRepo;
        private Mock<IBitacoraService> _mockBitacora;
        private Mock<ISessionService> _mockSession;
        private EquipoComputoService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IEquipoComputoRepository>();
            _mockBitacora = new Mock<IBitacoraService>();
            _mockSession = new Mock<ISessionService>();

            _mockSession.Setup(s => s.UsuarioActual)
                .Returns(new Usuario { Id = 1, NombreUsuario = "admin" });

            _service = new EquipoComputoService(_mockRepo.Object, _mockBitacora.Object, _mockSession.Object);
        }

        [TestMethod]
        public async Task AgregarAsync_ConDatosValidos_DebeAgregarEquipo()
        {
            // Arrange
            var equipo = new EquipoComputo
            {
                NumeroSerie = "ABC123",
                EtiquetaInventario = "INV-001",
                Marca = "Dell",
                Modelo = "Latitude 5410",
                Caracteristicas = "Core i5, 8GB RAM",
                TipoEquipoId = 1,
                EstadoId = 1,
                Activo = true
            };

            _mockRepo.Setup(r => r.ExistsByNumeroSerieAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.ExistsByEtiquetaInventarioAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.AgregarAsync(
                    It.IsAny<EquipoComputo>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipo);

            // Act
            var resultado = await _service.AgregarAsync(equipo);

            // Assert
            Assert.IsNotNull(resultado);
            _mockRepo.Verify(r => r.AgregarAsync(
                It.IsAny<EquipoComputo>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockBitacora.Verify(b => b.RegistrarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ActualizarAsync_ConDatosValidos_DebeActualizarEquipo()
        {
            // Arrange
            var equipo = new EquipoComputo
            {
                Id = 1,
                NumeroSerie = "ABC123",
                EtiquetaInventario = "INV-001",
                Marca = "Dell",
                Modelo = "Latitude 5410",
                Caracteristicas = "Core i5, 16GB RAM", // Actualizado
                TipoEquipoId = 1,
                EstadoId = 1,
                Activo = true
            };

            _mockRepo.Setup(r => r.ExistsByNumeroSerieAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.ExistsByEtiquetaInventarioAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _service.ActualizarAsync(equipo);

            // Assert
            _mockRepo.Verify(r => r.ActualizarAsync(
                It.IsAny<EquipoComputo>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockBitacora.Verify(b => b.RegistrarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}