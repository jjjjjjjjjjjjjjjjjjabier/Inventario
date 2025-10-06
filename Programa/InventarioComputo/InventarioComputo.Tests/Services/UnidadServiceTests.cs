using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Tests.Services
{
    [TestClass]
    public class UnidadServiceTests
    {
        private Mock<IUnidadRepository> _mockRepo;
        private UnidadService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IUnidadRepository>();
            _service = new UnidadService(_mockRepo.Object);
        }

        [TestMethod]
        public async Task BuscarAsync_DebeRetornarListaDeUnidades()
        {
            // Arrange
            var unidadesEsperadas = new List<Unidad>
            {
                new() { Id = 1, Nombre = "Pieza", Abreviatura = "Pz", Activo = true },
                new() { Id = 2, Nombre = "Kilogramo", Abreviatura = "Kg", Activo = true }
            };

            _mockRepo.Setup(r => r.BuscarAsync(
                    It.IsAny<string>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadesEsperadas);

            // Act
            var resultado = await _service.BuscarAsync("", true);

            // Assert
            Assert.AreEqual(unidadesEsperadas.Count, resultado.Count);
            Assert.AreEqual(unidadesEsperadas[0].Nombre, resultado[0].Nombre);
        }

        [TestMethod]
        public async Task GuardarAsync_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var unidad = new Unidad { Id = 0, Nombre = "Pieza", Abreviatura = "Pz", Activo = true };
            
            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(), 
                    It.IsAny<int?>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _service.GuardarAsync(unidad));
        }

        [TestMethod]
        public async Task GuardarAsync_ConDatosValidos_DebeGuardarUnidad()
        {
            // Arrange
            var unidad = new Unidad { Id = 0, Nombre = "Pieza", Abreviatura = "Pz", Activo = true };
            
            _mockRepo.Setup(r => r.ExisteNombreAsync(
                    It.IsAny<string>(), 
                    It.IsAny<int?>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
                
            _mockRepo.Setup(r => r.ExisteAbreviaturaAsync(
                    It.IsAny<string>(), 
                    It.IsAny<int?>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
                
            _mockRepo.Setup(r => r.GuardarAsync(
                    It.IsAny<Unidad>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidad);

            // Act
            var resultado = await _service.GuardarAsync(unidad);

            // Assert
            Assert.IsNotNull(resultado);
            _mockRepo.Verify(r => r.GuardarAsync(
                It.IsAny<Unidad>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
