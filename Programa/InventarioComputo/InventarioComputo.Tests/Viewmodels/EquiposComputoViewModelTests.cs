using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Tests.ViewModels
{
    [TestClass]
    public class EquiposComputoViewModelTests
    {
        private Mock<IEquipoComputoService> _mockService = null!;
        private Mock<IMovimientoService> _mockMovimientoService = null!;
        private Mock<IDialogService> _mockDialog = null!;
        private Mock<ISessionService> _mockSession = null!;
        private Mock<ILogger<EquiposComputoViewModel>> _mockLogger = null!;
        private EquiposComputoViewModel _viewModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IEquipoComputoService>();
            _mockMovimientoService = new Mock<IMovimientoService>();
            _mockDialog = new Mock<IDialogService>();
            _mockSession = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<EquiposComputoViewModel>>();

            // Usuario Administrador
            _mockSession.Setup(s => s.TieneRol("Administrador")).Returns(true);

            _viewModel = new EquiposComputoViewModel(
                _mockService.Object,
                _mockMovimientoService.Object,
                _mockDialog.Object,
                _mockSession.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeCargarEquiposEnColeccion()
        {
            // Arrange
            var equipos = new List<EquipoComputo>
            {
                new() { Id = 1, NumeroSerie = "ABC123", EtiquetaInventario = "INV-001", Marca = "Dell", Modelo = "Latitude 5420", TipoEquipoId = 1, EstadoId = 1, Activo = true },
                new() { Id = 2, NumeroSerie = "DEF456", EtiquetaInventario = "INV-002", Marca = "HP", Modelo = "EliteBook 840", TipoEquipoId = 1, EstadoId = 1, Activo = true }
            };

            _mockService
                .Setup(s => s.BuscarAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipos);

            // Act: usa el Command, no el método directamente
            await _viewModel.BuscarCommand.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(2, _viewModel.Equipos.Count);
            Assert.AreEqual("ABC123", _viewModel.Equipos[0].NumeroSerie);
            Assert.AreEqual("DEF456", _viewModel.Equipos[1].NumeroSerie);
        }

        [TestMethod]
        public void PuedeCrearEditar_ConUsuarioAdministrador_DebeRetornarTrue()
        {
            var puede = _viewModel.CrearCommand.CanExecute(null);
            Assert.IsTrue(puede);
        }

        [TestMethod]
        public void CanVerHistorial_SinEquipoSeleccionado_DebeRetornarFalse()
        {
            _viewModel.EquipoSeleccionado = null;

            // Evaluar el CanExecute del Command
            var puede = _viewModel.VerHistorialCommand.CanExecute(null);

            Assert.IsFalse(puede);
        }

        [TestMethod]
        public void CanVerHistorial_ConEquipoSeleccionado_DebeRetornarTrue()
        {
            _viewModel.EquipoSeleccionado = new EquipoComputo { Id = 1 };

            var puede = _viewModel.VerHistorialCommand.CanExecute(null);

            Assert.IsTrue(puede);
        }
    }
}