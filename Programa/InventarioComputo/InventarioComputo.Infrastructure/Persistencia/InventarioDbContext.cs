using InventarioComputo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioComputo.Infrastructure.Persistencia
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

        public DbSet<Estado> Estados { get; set; }
        public DbSet<Unidad> Unidades { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Zona> Zonas { get; set; }
        public DbSet<TipoEquipo> TiposEquipo { get; set; }
        public DbSet<EquipoComputo> EquiposComputo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuraciones explícitas y completas para cada tabla ---

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.ColorHex).HasMaxLength(9);
                entity.Property(e => e.Activo).IsRequired();
            });

            modelBuilder.Entity<Unidad>(entity =>
            {
                entity.ToTable("Unidades");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Nombre).IsUnique();
                entity.Property(u => u.Abreviatura).IsRequired().HasMaxLength(10);
                entity.HasIndex(u => u.Abreviatura).IsUnique();
                entity.Property(u => u.Activo).IsRequired();
            });

            modelBuilder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Activo).IsRequired();
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Areas");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Activo).IsRequired();
                entity.HasOne(a => a.Sede).WithMany(s => s.Areas).HasForeignKey(a => a.SedeId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Zona>(entity =>
            {
                entity.ToTable("Zonas");
                entity.HasKey(z => z.Id);
                entity.Property(z => z.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(z => z.Activo).IsRequired();
                entity.HasOne(z => z.Area).WithMany(a => a.Zonas).HasForeignKey(z => z.AreaId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TipoEquipo>(entity =>
            {
                entity.ToTable("TiposEquipo");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(t => t.Nombre).IsUnique();
                entity.Property(t => t.Activo).IsRequired();
            });

            modelBuilder.Entity<EquipoComputo>(entity =>
            {
                entity.ToTable("EquiposComputo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroSerie).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.NumeroSerie).IsUnique();
                entity.Property(e => e.EtiquetaInventario).HasMaxLength(100);
                entity.Property(e => e.Marca).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Modelo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Caracteristicas).HasMaxLength(500);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);

                entity.HasOne(e => e.TipoEquipo).WithMany().HasForeignKey(e => e.TipoEquipoId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Estado).WithMany().HasForeignKey(e => e.EstadoId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Zona).WithMany().HasForeignKey(e => e.ZonaId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}