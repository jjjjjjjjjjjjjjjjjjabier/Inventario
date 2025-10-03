using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventarioComputo.Infrastructure.Persistencia.Factories
{
    public class InventarioDbContextFactory : IDesignTimeDbContextFactory<InventarioDbContext>
    {
        public InventarioDbContext CreateDbContext(string[] args)
        {
            // Detecta appsettings.json de la UI para usar la misma cadena de conexión
            var cwd = Directory.GetCurrentDirectory();
            var uiDir = cwd;
            if (!File.Exists(Path.Combine(uiDir, "appsettings.json")))
            {
                var root = Directory.GetParent(cwd)?.FullName ?? cwd;
                var candidate = Path.Combine(root, "InventarioComputo.UI");
                if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                    uiDir = candidate;
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(uiDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var cs = config.GetConnectionString("DefaultConnection")
                     ?? "Server=.;Database=InventarioComputo;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

            var builder = new DbContextOptionsBuilder<InventarioDbContext>();
            builder.UseSqlServer(cs, sql =>
            {
                sql.MigrationsAssembly(typeof(InventarioDbContext).Assembly.FullName);
            });

            return new InventarioDbContext(builder.Options);
        }
    }
}