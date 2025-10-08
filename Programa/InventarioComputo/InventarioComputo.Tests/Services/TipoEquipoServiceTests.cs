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
    public class TipoEquipoServiceTests
    {
        private Mock<ITipoEquipoRepository> _mockRepo;
        private TipoEquipoService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ITipoEquipoRepository>();
            _service = new TipoEquipoService(_mockRepo.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeRetornarListaDeTipos()
        {
            // Arrange
            var tiposEsperados = new List<TipoEquipo>
            {
                new() { Id = 1, Nombre = "Laptop", Activo = true },
                new() { Id = 2, Nombre = "Desktop", Activo = true }
            };

            _mockRepo.Setup(r => r.BuscarAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposEsperados);

            // Act
            var resultado = await _service.BuscarAsync("", true);

            // Assert
            Assert.AreEqual(tiposEsperados.Count, resultado.Count);
            Assert.AreEqual(tiposEsperados[0].Nombre, resultado[0].Nombre);
        }

        [TestMethod]
        public async Task GuardarAsync_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var tipo = new TipoEquipo { Id = 0, Nombre = "Laptop", Activo = true };

            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _service.GuardarAsync(tipo));
        }

        [TestMethod]
        public async Task GuardarAsync_ConDatosValidos_DebeGuardarTipo()
        {
            // Arrange
            var tipo = new TipoEquipo { Id = 0, Nombre = "Laptop", Activo = true };

            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockRepo.Setup(r => r.GuardarAsync(
                    It.IsAny<TipoEquipo>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipo);

            // Act
            var resultado = await _service.GuardarAsync(tipo);

            // Assert
            Assert.IsNotNull(resultado);
            _mockRepo.Verify(r => r.GuardarAsync(
                It.IsAny<TipoEquipo>(),
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