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
    public class EstadosViewModelTests
    {
        private Mock<IEstadoService> _mockService = null!;
        private Mock<IDialogService> _mockDialog = null!;
        private Mock<ISessionService> _mockSession = null!;
        private Mock<ILogger<EstadosViewModel>> _mockLogger = null!;
        private EstadosViewModel _viewModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IEstadoService>();
            _mockDialog = new Mock<IDialogService>();
            _mockSession = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<EstadosViewModel>>();

            // Usuario Administrador
            _mockSession.Setup(s => s.TieneRol("Administrador")).Returns(true);

            _viewModel = new EstadosViewModel(
                _mockService.Object,
                _mockDialog.Object,
                _mockSession.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeCargarEstadosEnColeccion()
        {
            // Arrange
            var estados = new List<Estado>
            {
                new() { Id = 1, Nombre = "Nuevo", ColorHex = "#00C853", Activo = true },
                new() { Id = 2, Nombre = "En Uso", ColorHex = "#2196F3", Activo = true }
            };

            _mockService
                .Setup(s => s.BuscarAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(estados);

            // Act: ejecuta el comando directamente
            await _viewModel.BuscarCommand.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(2, _viewModel.Estados.Count);
            Assert.AreEqual("Nuevo", _viewModel.Estados[0].Nombre);
            Assert.AreEqual("#00C853", _viewModel.Estados[0].ColorHex);
        }

        [TestMethod]
        public void CrearCommand_UsuarioAdministrador_DebePoderEjecutarse()
        {
            // Act: Evalúa si puede ejecutar el comando
            var puede = _viewModel.CrearCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(puede);
        }

        [TestMethod]
        public void EditarCommand_SinEstadoSeleccionado_NoDebePoderEjecutarse()
        {
            // Arrange: No hay estado seleccionado
            _viewModel.EstadoSeleccionado = null;

            // Act
            var puede = _viewModel.EditarCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(puede);
        }

        [TestMethod]
        public void EditarCommand_ConEstadoSeleccionado_DebePoderEjecutarse()
        {
            // Arrange: hay estado seleccionado
            _viewModel.EstadoSeleccionado = new Estado { Id = 1 };

            // Act
            var puede = _viewModel.EditarCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(puede);
        }

        [TestMethod]
        public void EliminarCommand_SinEstadoSeleccionado_NoDebePoderEjecutarse()
        {
            // Arrange: No hay estado seleccionado
            _viewModel.EstadoSeleccionado = null;

            // Act
            var puede = _viewModel.EliminarCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(puede);
        }
    }
}