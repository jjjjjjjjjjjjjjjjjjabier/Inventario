using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEsEmpleadoSolamente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquiposComputo_Usuarios_UsuarioId",
                table: "EquiposComputo");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Usuarios_UsuarioAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Usuarios_UsuarioNuevoId",
                table: "HistorialMovimientos");

            migrationBuilder.RenameColumn(
                name: "UsuarioNuevoId",
                table: "HistorialMovimientos",
                newName: "EmpleadoNuevoId");

            migrationBuilder.RenameColumn(
                name: "UsuarioAnteriorId",
                table: "HistorialMovimientos",
                newName: "EmpleadoAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialMovimientos_UsuarioNuevoId",
                table: "HistorialMovimientos",
                newName: "IX_HistorialMovimientos_EmpleadoNuevoId");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialMovimientos_UsuarioAnteriorId",
                table: "HistorialMovimientos",
                newName: "IX_HistorialMovimientos_EmpleadoAnteriorId");

            migrationBuilder.AddColumn<bool>(
                name: "EsEmpleadoSolamente",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmpleadoId",
                table: "EquiposComputo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Administradores");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Soporte");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 3, "Consulta" });

            migrationBuilder.CreateIndex(
                name: "IX_EquiposComputo_EmpleadoId",
                table: "EquiposComputo",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquiposComputo_Empleados_EmpleadoId",
                table: "EquiposComputo",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EquiposComputo_Usuarios_UsuarioId",
                table: "EquiposComputo",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Empleados_EmpleadoAnteriorId",
                table: "HistorialMovimientos",
                column: "EmpleadoAnteriorId",
                principalTable: "Empleados",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Empleados_EmpleadoNuevoId",
                table: "HistorialMovimientos",
                column: "EmpleadoNuevoId",
                principalTable: "Empleados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquiposComputo_Empleados_EmpleadoId",
                table: "EquiposComputo");

            migrationBuilder.DropForeignKey(
                name: "FK_EquiposComputo_Usuarios_UsuarioId",
                table: "EquiposComputo");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Empleados_EmpleadoAnteriorId",
                table: "HistorialMovimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialMovimientos_Empleados_EmpleadoNuevoId",
                table: "HistorialMovimientos");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_EquiposComputo_EmpleadoId",
                table: "EquiposComputo");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "EsEmpleadoSolamente",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EmpleadoId",
                table: "EquiposComputo");

            migrationBuilder.RenameColumn(
                name: "EmpleadoNuevoId",
                table: "HistorialMovimientos",
                newName: "UsuarioNuevoId");

            migrationBuilder.RenameColumn(
                name: "EmpleadoAnteriorId",
                table: "HistorialMovimientos",
                newName: "UsuarioAnteriorId");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialMovimientos_EmpleadoNuevoId",
                table: "HistorialMovimientos",
                newName: "IX_HistorialMovimientos_UsuarioNuevoId");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialMovimientos_EmpleadoAnteriorId",
                table: "HistorialMovimientos",
                newName: "IX_HistorialMovimientos_UsuarioAnteriorId");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Administrador");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Consulta");

            migrationBuilder.AddForeignKey(
                name: "FK_EquiposComputo_Usuarios_UsuarioId",
                table: "EquiposComputo",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Usuarios_UsuarioAnteriorId",
                table: "HistorialMovimientos",
                column: "UsuarioAnteriorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialMovimientos_Usuarios_UsuarioNuevoId",
                table: "HistorialMovimientos",
                column: "UsuarioNuevoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
