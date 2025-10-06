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
    public class UnidadesViewModelTests
    {
        private Mock<IUnidadService> _mockService = null!;
        private Mock<IDialogService> _mockDialog = null!;
        private Mock<ISessionService> _mockSession = null!;
        private Mock<ILogger<UnidadesViewModel>> _mockLogger = null!;
        private UnidadesViewModel _viewModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IUnidadService>();
            _mockDialog = new Mock<IDialogService>();
            _mockSession = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<UnidadesViewModel>>();

            // Usuario con rol Administrador
            _mockSession.Setup(s => s.TieneRol("Administrador")).Returns(true);

            _viewModel = new UnidadesViewModel(
                _mockService.Object,
                _mockDialog.Object,
                _mockSession.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeCargarUnidadesEnColeccion()
        {
            // Arrange
            var unidades = new List<Unidad>
            {
                new() { Id = 1, Nombre = "Pieza", Abreviatura = "Pz", Activo = true },
                new() { Id = 2, Nombre = "Kilogramo", Abreviatura = "Kg", Activo = true }
            };

            _mockService
                .Setup(s => s.BuscarAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidades);

            // Act: ejecutar el Command generado por [RelayCommand]
            await _viewModel.BuscarCommand.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(2, _viewModel.Unidades.Count);
            Assert.AreEqual("Pieza", _viewModel.Unidades[0].Nombre);
            Assert.AreEqual("Kilogramo", _viewModel.Unidades[1].Nombre);
        }

        [TestMethod]
        public void PuedeCrearEditar_ConUsuarioAdministrador_DebeRetornarTrue()
        {
            // En lugar de llamar al helper privado, validamos CanExecute del Command
            var puede = _viewModel.CrearCommand.CanExecute(null);
            Assert.IsTrue(puede);
        }

        [TestMethod]
        public void PuedeCrearEditar_ConUsuarioNoAdministrador_DebeRetornarFalse()
        {
            // Arrange: instancia con sesión sin rol Administrador
            var mockSessionNoAdmin = new Mock<ISessionService>();
            mockSessionNoAdmin.Setup(s => s.TieneRol("Administrador")).Returns(false);

            var vm = new UnidadesViewModel(
                _mockService.Object,
                _mockDialog.Object,
                mockSessionNoAdmin.Object,
                _mockLogger.Object);

            // Act
            var puede = vm.CrearCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(puede);
        }
    }
}