using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Tests.Services
{
    [TestClass]
    public class EstadoServiceTests
    {
        private Mock<IEstadoRepository> _mockRepo;
        private EstadoService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IEstadoRepository>();
            _service = new EstadoService(_mockRepo.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeRetornarListaDeEstados()
        {
            // Arrange
            var estadosEsperados = new List<Estado>
            {
                new() { Id = 1, Nombre = "Nuevo", ColorHex = "#00C853", Activo = true },
                new() { Id = 2, Nombre = "En Uso", ColorHex = "#2196F3", Activo = true }
            };

            _mockRepo.Setup(r => r.BuscarAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(estadosEsperados);

            // Act
            var resultado = await _service.BuscarAsync("", true);

            // Assert
            Assert.AreEqual(estadosEsperados.Count, resultado.Count);
            Assert.AreEqual(estadosEsperados[0].Nombre, resultado[0].Nombre);
            Assert.AreEqual(estadosEsperados[0].ColorHex, resultado[0].ColorHex);
        }

        [TestMethod]
        public async Task GuardarAsync_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var estado = new Estado { Id = 0, Nombre = "Nuevo", ColorHex = "#00C853", Activo = true };

            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _service.GuardarAsync(estado));
        }

        [TestMethod]
        public async Task GuardarAsync_ConDatosValidos_DebeGuardarEstado()
        {
            // Arrange
            var estado = new Estado { Id = 0, Nombre = "Nuevo", ColorHex = "#00C853", Activo = true };

            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.GuardarAsync(
                    It.IsAny<Estado>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(estado);

            // Act
            var resultado = await _service.GuardarAsync(estado);

            // Assert
            Assert.IsNotNull(resultado);
            _mockRepo.Verify(r => r.GuardarAsync(
                It.IsAny<Estado>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task EliminarAsync_DebeInvocarRepositorio()
        {
            // Arrange
            var id = 1;

            // Act
            await _service.EliminarAsync(id);

            // Assert
            _mockRepo.Verify(r => r.EliminarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}