using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Agrega_Campo_Activo_A_EquipoComputo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ZonaId",
                table: "EquiposComputo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "EquiposComputo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "EquiposComputo");

            migrationBuilder.AlterColumn<int>(
                name: "ZonaId",
                table: "EquiposComputo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
