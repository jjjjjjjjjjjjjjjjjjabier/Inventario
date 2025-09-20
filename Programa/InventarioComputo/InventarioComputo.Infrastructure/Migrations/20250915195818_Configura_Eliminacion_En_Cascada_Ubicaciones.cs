using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Configura_Eliminacion_En_Cascada_Ubicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zonas_Areas_AreaId",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_Zonas_AreaId_Nombre",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_Sedes_Nombre",
                table: "Sedes");

            migrationBuilder.DropIndex(
                name: "IX_Areas_SedeId_Nombre",
                table: "Areas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Zonas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Zonas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Sedes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Areas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.CreateIndex(
                name: "IX_Zonas_AreaId",
                table: "Zonas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_SedeId",
                table: "Areas",
                column: "SedeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zonas_Areas_AreaId",
                table: "Zonas",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zonas_Areas_AreaId",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_Zonas_AreaId",
                table: "Zonas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_SedeId",
                table: "Areas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Zonas",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Zonas",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Sedes",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Areas",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Zonas_AreaId_Nombre",
                table: "Zonas",
                columns: new[] { "AreaId", "Nombre" },
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

            migrationBuilder.AddForeignKey(
                name: "FK_Zonas_Areas_AreaId",
                table: "Zonas",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
