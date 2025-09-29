using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIndicesUnicosUbicacionesYCatalogos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zonas_AreaId",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_SedeId",
                table: "Areas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Zonas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposEquipo",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Areas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Zonas_AreaId_Nombre",
                table: "Zonas",
                columns: new[] { "AreaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposEquipo_Nombre",
                table: "TiposEquipo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Nombre",
                table: "Sedes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_SedeId_Nombre",
                table: "Areas",
                columns: new[] { "SedeId", "Nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zonas_AreaId_Nombre",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_TiposEquipo_Nombre",
                table: "TiposEquipo");

            migrationBuilder.DropIndex(
                name: "IX_Sedes_Nombre",
                table: "Sedes");

            migrationBuilder.DropIndex(
                name: "IX_Areas_SedeId_Nombre",
                table: "Areas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Zonas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposEquipo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Zonas_AreaId",
                table: "Zonas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_SedeId",
                table: "Areas",
                column: "SedeId");
        }
    }
}
