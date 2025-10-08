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
    public class HistorialMovimientoServiceTests
    {
        private Mock<IHistorialMovimientoRepository> _mockRepo;
        private Mock<IEquipoComputoRepository> _mockEquipoRepo;
        private HistorialMovimientoService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IHistorialMovimientoRepository>();
            _mockEquipoRepo = new Mock<IEquipoComputoRepository>();
            _service = new HistorialMovimientoService(_mockRepo.Object, _mockEquipoRepo.Object);
        }

        [TestMethod]
        public async Task ObtenerHistorialEquipoAsync_DebeRetornarHistorial()
        {
            // Arrange
            var equipoId = 1;
            var historialEsperado = new List<HistorialMovimiento>
            {
                new() { Id = 1, EquipoComputoId = equipoId, FechaMovimiento = DateTime.Now.AddDays(-5) },
                new() { Id = 2, EquipoComputoId = equipoId, FechaMovimiento = DateTime.Now }
            };

            _mockRepo.Setup(r => r.ObtenerHistorialEquipoAsync(
                    equipoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(historialEsperado);

            // Act
            var resultado = await _service.ObtenerHistorialEquipoAsync(equipoId);

            // Assert
            Assert.AreEqual(historialEsperado.Count, resultado.Count);
            Assert.AreEqual(historialEsperado[0].Id, resultado[0].Id);
        }

        [TestMethod]
        public async Task RegistrarMovimientoAsync_ConDatosValidos_DebeAgregarHistorial()
        {
            // Arrange
            var equipoId = 1;
            var empleadoNuevoId = 10;
            var zonaNuevaId = 5;
            var usuarioResponsableId = 3;
            var motivo = "Cambio de ubicación";

            var equipo = new EquipoComputo
            {
                Id = equipoId,
                EmpleadoId = 8, // Empleado anterior
                ZonaId = 2,     // Zona anterior
                TipoEquipoId = 1,
                EstadoId = 1,
                AreaId = 1,
                SedeId = 1
            };

            _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(
                    equipoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipo);

            _mockRepo.Setup(r => r.AgregarAsync(
                    It.IsAny<HistorialMovimiento>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HistorialMovimiento hm, CancellationToken ct) => hm);

            // Act
            await _service.RegistrarMovimientoAsync(equipoId, empleadoNuevoId, zonaNuevaId, motivo, usuarioResponsableId);

            // Assert
            _mockRepo.Verify(r => r.AgregarAsync(
                It.Is<HistorialMovimiento>(h =>
                    h.EquipoComputoId == equipoId &&
                    h.EmpleadoAnteriorId == equipo.EmpleadoId &&
                    h.ZonaAnteriorId == equipo.ZonaId &&
                    h.EmpleadoNuevoId == empleadoNuevoId &&
                    h.ZonaNuevaId == zonaNuevaId &&
                    h.Motivo == motivo &&
                    h.UsuarioResponsableId == usuarioResponsableId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task RegistrarMovimientoAsync_ConMotivoVacio_DebeLanzarExcepcion()
        {
            // Arrange
            var equipoId = 1;
            var empleadoNuevoId = 10;
            var zonaNuevaId = 5;
            var usuarioResponsableId = 3;
            var motivoVacio = "";

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await _service.RegistrarMovimientoAsync(
                    equipoId, empleadoNuevoId, zonaNuevaId, motivoVacio, usuarioResponsableId));
        }

        [TestMethod]
        public async Task RegistrarMovimientoAsync_EquipoNoExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var equipoId = 999; // ID que no existe
            var empleadoNuevoId = 10;
            var zonaNuevaId = 5;
            var usuarioResponsableId = 3;
            var motivo = "Cambio de ubicación";

            // Simular que el equipo no existe
            _mockEquipoRepo.Setup(r => r.ObtenerPorIdAsync(
                    equipoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EquipoComputo)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _service.RegistrarMovimientoAsync(
                    equipoId, empleadoNuevoId, zonaNuevaId, motivo, usuarioResponsableId));
        }
    }
}