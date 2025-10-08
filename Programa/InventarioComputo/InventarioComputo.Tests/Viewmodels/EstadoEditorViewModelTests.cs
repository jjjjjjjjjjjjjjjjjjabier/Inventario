using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace InventarioComputo.Tests.ViewModels
{
    [TestClass]
    public class EstadoEditorViewModelTests
    {
        private Mock<IEstadoService> _mockService;
        private Mock<IDialogService> _mockDialog;
        private Mock<ILogger<EstadoEditorViewModel>> _mockLogger;
        private EstadoEditorViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IEstadoService>();
            _mockDialog = new Mock<IDialogService>();
            _mockLogger = new Mock<ILogger<EstadoEditorViewModel>>();

            _viewModel = new EstadoEditorViewModel(
                _mockService.Object,
                _mockDialog.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public void SetEstado_ConEstadoExistente_DebeCargarDatosYCambiarTitulo()
        {
            // Arrange
            var estado = new Estado
            {
                Id = 5,
                Nombre = "En Reparación",
                Descripcion = "Equipo en mantenimiento",
                ColorHex = "#FFC107",
                Activo = true
            };

            // Act
            _viewModel.SetEstado(estado);

            // Assert
            Assert.AreEqual("Editar Estado", _viewModel.Titulo);
            Assert.AreEqual("En Reparación", _viewModel.Nombre);
            Assert.AreEqual("Equipo en mantenimiento", _viewModel.Descripcion);
            Assert.AreEqual("#FFC107", _viewModel.ColorHex);
            Assert.IsTrue(_viewModel.Activo);
        }

        [TestMethod]
        public void SetEstado_ConEstadoNuevo_DebeManteneTituloNuevoEstado()
        {
            // Arrange
            var estado = new Estado
            {
                Id = 0, // ID 0 indica nuevo
                Nombre = "Obsoleto",
                Descripcion = "Equipo descontinuado",
                ColorHex = "#9E9E9E",
                Activo = false
            };

            // Act
            _viewModel.SetEstado(estado);

            // Assert
            Assert.AreEqual("Nuevo Estado", _viewModel.Titulo);
            Assert.AreEqual("Obsoleto", _viewModel.Nombre);
        }

        [TestMethod]
        public async Task GuardarAsync_SinNombre_DebeMostrarError()
        {
            // Arrange
            _viewModel.SetEstado(new Estado { Id = 0 });
            _viewModel.Nombre = ""; // Nombre vacío debe causar error

            // Act
            await _viewModel.GuardarCommand.ExecuteAsync(null);

            // Assert
            _mockDialog.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
            _mockService.Verify(s => s.GuardarAsync(It.IsAny<Estado>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task GuardarAsync_SinColor_DebeMostrarError()
        {
            // Arrange
            _viewModel.SetEstado(new Estado { Id = 0 });
            _viewModel.Nombre = "Estado Válido";
            _viewModel.ColorHex = ""; // Color vacío debe causar error

            // Act
            await _viewModel.GuardarCommand.ExecuteAsync(null);

            // Assert
            _mockDialog.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
            _mockService.Verify(s => s.GuardarAsync(It.IsAny<Estado>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task GuardarAsync_ConColorInvalido_DebeMostrarError()
        {
            // Arrange
            _viewModel.SetEstado(new Estado { Id = 0 });
            _viewModel.Nombre = "Estado Válido";
            _viewModel.ColorHex = "NO-HEXADECIMAL"; // Color con formato inválido

            // Act
            await _viewModel.GuardarCommand.ExecuteAsync(null);

            // Assert
            _mockDialog.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
            _mockService.Verify(s => s.GuardarAsync(It.IsAny<Estado>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task GuardarAsync_ConDatosValidos_DebeGuardarEstado()
        {
            // Arrange
            var estado = new Estado { Id = 0 };
            _viewModel.SetEstado(estado);
            _viewModel.Nombre = "Estado Nuevo";
            _viewModel.Descripcion = "Descripción de prueba";
            _viewModel.ColorHex = "#FF5722";
            _viewModel.Activo = true;

            _mockService.Setup(s => s.GuardarAsync(It.IsAny<Estado>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Estado e, CancellationToken ct) => e);

            // Act
            await _viewModel.GuardarCommand.ExecuteAsync(null);

            // Assert
            _mockService.Verify(s => s.GuardarAsync(
                It.Is<Estado>(e => 
                    e.Nombre == "Estado Nuevo" && 
                    e.ColorHex == "#FF5722"), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            _mockDialog.Verify(d => d.ShowInfo(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GuardarAsync_ConNombreDuplicado_DebeMostrarError()
        {
            // Arrange
            _viewModel.SetEstado(new Estado { Id = 0 });
            _viewModel.Nombre = "Estado Duplicado";
            _viewModel.ColorHex = "#FF5722";

            _mockService.Setup(s => s.GuardarAsync(
                It.IsAny<Estado>(), 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Ya existe un estado con ese nombre"));

            // Act
            await _viewModel.GuardarCommand.ExecuteAsync(null);

            // Assert
            _mockService.Verify(s => s.GuardarAsync(It.IsAny<Estado>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDialog.Verify(d => d.ShowError("Ya existe un estado con ese nombre"), Times.Once);
        }
    }
}