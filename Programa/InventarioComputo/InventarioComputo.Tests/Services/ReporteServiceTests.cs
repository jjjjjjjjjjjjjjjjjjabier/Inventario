using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Infrastructure.Services;
using Microsoft.Extensions.Logging;
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
    public class ReporteServiceTests
    {
        private Mock<IEquipoComputoRepository> _mockRepo;
        private Mock<ILogger<ReporteService>> _mockLogger;
        private ReporteService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IEquipoComputoRepository>();
            _mockLogger = new Mock<ILogger<ReporteService>>();
            _service = new ReporteService(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task ObtenerEquiposFiltradosAsync_DebeRetornarListaDeRepositorio()
        {
            // Arrange
            var filtro = new FiltroReporteDTO();
            var equipos = new List<EquipoComputo> 
            {
                new EquipoComputo { Id = 1, NumeroSerie = "123", Marca = "Dell" },
                new EquipoComputo { Id = 2, NumeroSerie = "456", Marca = "HP" }
            };
            
            _mockRepo.Setup(r => r.ObtenerParaReporteAsync(
                It.IsAny<FiltroReporteDTO>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipos);

            // Act
            var resultado = await _service.ObtenerEquiposFiltradosAsync(filtro);

            // Assert
            Assert.AreEqual(2, resultado.Count);
            _mockRepo.Verify(r => r.ObtenerParaReporteAsync(filtro, default), Times.Once);
        }

        [TestMethod]
        public async Task ObtenerEquiposDTOFiltradosAsync_DebeMapeaADTOsCorrectamente()
        {
            // Arrange
            var tipoEquipo = new TipoEquipo { Id = 1, Nombre = "Laptop" };
            var estado = new Estado { Id = 1, Nombre = "Operativo" };
            var empleado = new Empleado { Id = 1, NombreCompleto = "Juan Pérez" };
            var sede = new Sede { Id = 1, Nombre = "Planta 1" };
            var area = new Area { Id = 1, Nombre = "TI", Sede = sede, SedeId = sede.Id };
            var zona = new Zona { Id = 1, Nombre = "Sala de Servidores", Area = area, AreaId = area.Id };

            var equipos = new List<EquipoComputo> 
            {
                new EquipoComputo 
                { 
                    Id = 1, 
                    NumeroSerie = "SN001", 
                    EtiquetaInventario = "INV001", 
                    Marca = "Dell", 
                    Modelo = "Latitude", 
                    FechaAdquisicion = new DateTime(2022, 1, 15),
                    TipoEquipo = tipoEquipo,
                    TipoEquipoId = tipoEquipo.Id,
                    Estado = estado,
                    EstadoId = estado.Id,
                    Empleado = empleado,
                    EmpleadoId = empleado.Id,
                    Zona = zona,
                    ZonaId = zona.Id
                }
            };
            
            _mockRepo.Setup(r => r.ObtenerParaReporteAsync(
                It.IsAny<FiltroReporteDTO>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(equipos);

            // Act
            var resultado = await _service.ObtenerEquiposDTOFiltradosAsync(new FiltroReporteDTO());

            // Assert
            Assert.AreEqual(1, resultado.Count);
            var dto = resultado.First();
            Assert.AreEqual("SN001", dto.NumeroSerie);
            Assert.AreEqual("INV001", dto.EtiquetaInventario);
            Assert.AreEqual("Dell", dto.Marca);
            Assert.AreEqual("Latitude", dto.Modelo);
            Assert.AreEqual("Laptop", dto.TipoEquipo);
            Assert.AreEqual("Operativo", dto.Estado);
            Assert.AreEqual("Juan Pérez", dto.UsuarioAsignado);
            Assert.AreEqual("Planta 1 / TI / Sala de Servidores", dto.Ubicacion);
            Assert.AreEqual(new DateTime(2022, 1, 15), dto.FechaAdquisicion);
        }

        [TestMethod]
        public async Task ExportarExcelAsync_DebeGenerarArchivoExcel()
        {
            // Arrange
            var datos = new List<ReporteEquipoDTO>
            {
                new ReporteEquipoDTO
                {
                    Id = 1,
                    NumeroSerie = "SN001",
                    EtiquetaInventario = "INV001",
                    Marca = "Dell",
                    Modelo = "Latitude",
                    TipoEquipo = "Laptop",
                    Estado = "Operativo",
                    UsuarioAsignado = "Juan Pérez",
                    Ubicacion = "Planta 1 / TI / Oficina",
                    FechaAdquisicion = new DateTime(2022, 1, 15)
                }
            };

            // Act
            var bytes = await _service.ExportarExcelAsync(datos);

            // Assert
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
        }

        [TestMethod]
        public async Task ExportarPDFAsync_DebeGenerarArchivoPDF()
        {
            // Arrange
            var datos = new List<ReporteEquipoDTO>
            {
                new ReporteEquipoDTO
                {
                    Id = 1,
                    NumeroSerie = "SN001",
                    EtiquetaInventario = "INV001",
                    Marca = "Dell",
                    Modelo = "Latitude",
                    TipoEquipo = "Laptop",
                    Estado = "Operativo",
                    UsuarioAsignado = "Juan Pérez",
                    Ubicacion = "Planta 1 / TI / Oficina",
                    FechaAdquisicion = new DateTime(2022, 1, 15)
                }
            };

            // Act
            var bytes = await _service.ExportarPDFAsync(datos);

            // Assert
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
        }
    }
}