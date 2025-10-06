using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioComputo.Infrastructure.Migrations
{
    public partial class UniqueEmpleadoNombre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT 1 FROM sys.indexes 
                              WHERE name = 'IX_Empleados_NombreCompleto' AND object_id = OBJECT_ID('dbo.Empleados'))
                BEGIN
                    CREATE UNIQUE INDEX IX_Empleados_NombreCompleto ON Empleados (NombreCompleto)
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS(SELECT 1 FROM sys.indexes 
                          WHERE name = 'IX_Empleados_NombreCompleto' AND object_id = OBJECT_ID('dbo.Empleados'))
                BEGIN
                    DROP INDEX IX_Empleados_NombreCompleto ON Empleados
                END
            ");
        }
    }
}