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
    public class TiposEquipoViewModelTests
    {
        private Mock<ITipoEquipoService> _mockService = null!;
        private Mock<IDialogService> _mockDialog = null!;
        private Mock<ISessionService> _mockSession = null!;
        private Mock<ILogger<TiposEquipoViewModel>> _mockLogger = null!;
        private TiposEquipoViewModel _viewModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<ITipoEquipoService>();
            _mockDialog = new Mock<IDialogService>();
            _mockSession = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<TiposEquipoViewModel>>();

            // Usuario Administrador
            _mockSession.Setup(s => s.TieneRol("Administrador")).Returns(true);

            _viewModel = new TiposEquipoViewModel(
                _mockService.Object,
                _mockDialog.Object,
                _mockSession.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeCargarTiposEnColeccion()
        {
            // Arrange
            var tipos = new List<TipoEquipo>
            {
                new() { Id = 1, Nombre = "Laptop", Activo = true },
                new() { Id = 2, Nombre = "Desktop", Activo = true }
            };

            _mockService
                .Setup(s => s.BuscarAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipos);

            // Act: ejecuta el comando directamente
            await _viewModel.BuscarCommand.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(2, _viewModel.TiposEquipo.Count);
            Assert.AreEqual("Laptop", _viewModel.TiposEquipo[0].Nombre);
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
        public void EditarCommand_SinTipoSeleccionado_NoDebePoderEjecutarse()
        {
            // Arrange: No hay tipo seleccionado
            _viewModel.TipoEquipoSeleccionado = null;

            // Act
            var puede = _viewModel.EditarCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(puede);
        }

        [TestMethod]
        public void EditarCommand_ConTipoSeleccionado_DebePoderEjecutarse()
        {
            // Arrange: hay tipo seleccionado
            _viewModel.TipoEquipoSeleccionado = new TipoEquipo { Id = 1 };

            // Act
            var puede = _viewModel.EditarCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(puede);
        }

        [TestMethod]
        public void EliminarCommand_SinTipoSeleccionado_NoDebePoderEjecutarse()
        {
            // Arrange: No hay tipo seleccionado
            _viewModel.TipoEquipoSeleccionado = null;

            // Act
            var puede = _viewModel.EliminarCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(puede);
        }
    }
}