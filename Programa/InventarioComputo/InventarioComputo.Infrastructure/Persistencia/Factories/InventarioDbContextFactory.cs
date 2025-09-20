using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InventarioComputo.Infrastructure.Persistencia.Factories
{
    public class InventarioDbContextFactory : IDesignTimeDbContextFactory<InventarioDbContext>
    {
        public InventarioDbContext CreateDbContext(string[] args)
        {
            // Esta lógica busca la carpeta del proyecto UI para encontrar el appsettings.json
            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\InventarioComputo.UI"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InventarioDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new InventarioDbContext(optionsBuilder.Options);
        }
    }
}