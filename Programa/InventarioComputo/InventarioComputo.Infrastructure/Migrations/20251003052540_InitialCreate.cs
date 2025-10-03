using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BitacoraEventos",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: false),
                    UsuarioResponsableId = table.Column<int>(type: "int", nullable: true),
                    Detalles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraEventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposEquipo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEquipo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unidades",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abreviatura = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SedeId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "dbo",
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                schema: "dbo",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zonas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zonas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "dbo",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquiposComputo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroSerie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EtiquetaInventario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Caracteristicas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaAdquisicion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    TipoEquipoId = table.Column<int>(type: "int", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    SedeId = table.Column<int>(type: "int", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    ZonaId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquiposComputo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "dbo",
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalSchema: "dbo",
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalSchema: "dbo",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "dbo",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquiposComputo_TiposEquipo_TipoEquipoId",
                        column: x => x.TipoEquipoId,
                        principalSchema: "dbo",
                        principalTable: "TiposEquipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EquiposComputo_Zonas_ZonaId",
                        column: x => x.ZonaId,
                        principalSchema: "dbo",
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HistorialMovimientos",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipoComputoId = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioAnteriorId = table.Column<int>(type: "int", nullable: true),
                    SedeAnteriorId = table.Column<int>(type: "int", nullable: true),
                    AreaAnteriorId = table.Column<int>(type: "int", nullable: true),
                    ZonaAnteriorId = table.Column<int>(type: "int", nullable: true),
                    UsuarioNuevoId = table.Column<int>(type: "int", nullable: true),
                    SedeNuevaId = table.Column<int>(type: "int", nullable: true),
                    AreaNuevaId = table.Column<int>(type: "int", nullable: true),
                    ZonaNuevaId = table.Column<int>(type: "int", nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsuarioResponsableId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Areas_AreaAnteriorId",
                        column: x => x.AreaAnteriorId,
                        principalSchema: "dbo",
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Areas_AreaNuevaId",
                        column: x => x.AreaNuevaId,
                        principalSchema: "dbo",
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_EquiposComputo_EquipoComputoId",
                        column: x => x.EquipoComputoId,
                        principalSchema: "dbo",
                        principalTable: "EquiposComputo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Sedes_SedeAnteriorId",
                        column: x => x.SedeAnteriorId,
                        principalSchema: "dbo",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Sedes_SedeNuevaId",
                        column: x => x.SedeNuevaId,
                        principalSchema: "dbo",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Usuarios_UsuarioAnteriorId",
                        column: x => x.UsuarioAnteriorId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Usuarios_UsuarioNuevoId",
                        column: x => x.UsuarioNuevoId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Usuarios_UsuarioResponsableId",
                        column: x => x.UsuarioResponsableId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Zonas_ZonaAnteriorId",
                        column: x => x.ZonaAnteriorId,
                        principalSchema: "dbo",
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Zonas_ZonaNuevaId",
                        column: x => x.ZonaNuevaId,
                        principalSchema: "dbo",
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Consulta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_SedeId_Nombre",
                schema: "dbo",
                table: "Areas",
                columns: new[] { "SedeId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_AreaId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_EmpleadoId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_EstadoId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_EtiquetaInventario",
                schema: "dbo",
                table: "EquiposComputo",
                column: "EtiquetaInventario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_NumeroSerie",
                schema: "dbo",
                table: "EquiposComputo",
                column: "NumeroSerie",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_SedeId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_TipoEquipoId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "TipoEquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_UsuarioId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_ZonaId",
                schema: "dbo",
                table: "EquiposComputo",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Estados_Nombre",
                schema: "dbo",
                table: "Estados",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_AreaAnteriorId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "AreaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_AreaNuevaId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "AreaNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_EquipoComputoId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "EquipoComputoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_SedeAnteriorId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "SedeAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_SedeNuevaId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "SedeNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_UsuarioAnteriorId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "UsuarioAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_UsuarioNuevoId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "UsuarioNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_UsuarioResponsableId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "UsuarioResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ZonaAnteriorId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "ZonaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ZonaNuevaId",
                schema: "dbo",
                table: "HistorialMovimientos",
                column: "ZonaNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                schema: "dbo",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Nombre",
                schema: "dbo",
                table: "Sedes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposEquipo_Nombre",
                schema: "dbo",
                table: "TiposEquipo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_Abreviatura",
                schema: "dbo",
                table: "Unidades",
                column: "Abreviatura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unidades_Nombre",
                schema: "dbo",
                table: "Unidades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolId",
                schema: "dbo",
                table: "UsuarioRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                schema: "dbo",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zonas_AreaId_Nombre",
                schema: "dbo",
                table: "Zonas",
                columns: new[] { "AreaId", "Nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BitacoraEventos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HistorialMovimientos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Unidades",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UsuarioRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EquiposComputo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Empleados",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Estados",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TiposEquipo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Zonas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Sedes",
                schema: "dbo");
        }
    }
}
