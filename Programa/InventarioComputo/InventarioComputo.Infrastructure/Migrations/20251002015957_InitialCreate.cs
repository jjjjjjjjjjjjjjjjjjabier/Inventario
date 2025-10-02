using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UsuarioRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AreaAnteriorId",
                table: "HistorialMovimientos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaNuevaId",
                table: "HistorialMovimientos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SedeAnteriorId",
                table: "HistorialMovimientos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SedeNuevaId",
                table: "HistorialMovimientos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "EquiposComputo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Costo",
                table: "EquiposComputo",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SedeId",
                table: "EquiposComputo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BitacoraEventos",
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

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_AreaAnteriorId",
                table: "HistorialMovimientos",
                column: "AreaAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_AreaNuevaId",
                table: "HistorialMovimientos",
                column: "AreaNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_SedeAnteriorId",
                table: "HistorialMovimientos",
                column: "SedeAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_SedeNuevaId",
                table: "HistorialMovimientos",
                column: "SedeNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_AreaId",
                table: "EquiposComputo",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_SedeId",
                table: "EquiposComputo",
                column: "SedeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquiposComputo_Areas_AreaId",
                table: "EquiposComputo",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EquiposComputo_Sedes_SedeId",
                table: "EquiposComputo",
                column: "SedeId",
                principalTable: "Sedes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Areas_AreaAnteriorId",
                table: "HistorialMovimientos",
                column: "AreaAnteriorId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Areas_AreaNuevaId",
                table: "HistorialMovimientos",
                column: "AreaNuevaId",
                principalTable: "Areas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Sedes_SedeAnteriorId",
                table: "HistorialMovimientos",
                column: "SedeAnteriorId",
                principalTable: "Sedes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Sedes_SedeNuevaId",
                table: "HistorialMovimientos",
                column: "SedeNuevaId",
                principalTable: "Sedes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquiposComputo_Areas_AreaId",
                table: "EquiposComputo");

            migrationBuilder.DropForeignKey(
                name: "FK_EquiposComputo_Sedes_SedeId",
                table: "EquiposComputo");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Areas_AreaAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Areas_AreaNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Sedes_SedeAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Sedes_SedeNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropTable(
                name: "BitacoraEventos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMovimientos_AreaAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMovimientos_AreaNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMovimientos_SedeAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialMovimientos_SedeNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_EquiposComputo_AreaId",
                table: "EquiposComputo");

            migrationBuilder.DropIndex(
                name: "IX_EquiposComputo_SedeId",
                table: "EquiposComputo");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsuarioRoles");

            migrationBuilder.DropColumn(
                name: "AreaAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropColumn(
                name: "AreaNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropColumn(
                name: "SedeAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropColumn(
                name: "SedeNuevaId",
                table: "HistorialMovimientos");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "EquiposComputo");

            migrationBuilder.DropColumn(
                name: "Costo",
                table: "EquiposComputo");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "EquiposComputo");
        }
    }
}
