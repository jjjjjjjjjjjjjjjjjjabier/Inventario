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
        
        // Propiedades faltantes para la gestión de usuarios y seguridad
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<HistorialMovimiento> HistorialMovimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuraciones explícitas y completas para cada tabla ---

            // Unidad
            modelBuilder.Entity<Unidad>(entity =>
            {
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasIndex(e => e.Abreviatura).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Abreviatura).IsRequired().HasMaxLength(10);
            });

            // EquipoComputo
            modelBuilder.Entity<EquipoComputo>(entity =>
            {
                entity.ToTable("EquiposComputo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroSerie).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.NumeroSerie).IsUnique();
                entity.Property(e => e.EtiquetaInventario).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.EtiquetaInventario).IsUnique();
                entity.Property(e => e.Marca).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Modelo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Caracteristicas).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);
                entity.Property(e => e.Activo).IsRequired();
                
                // Relaciones
                entity.HasOne(e => e.TipoEquipo)
                      .WithMany()
                      .HasForeignKey(e => e.TipoEquipoId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(e => e.Estado)
                      .WithMany()
                      .HasForeignKey(e => e.EstadoId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(e => e.Zona)
                      .WithMany()
                      .HasForeignKey(e => e.ZonaId)
                      .OnDelete(DeleteBehavior.SetNull);
                      
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.EquiposAsignados)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Estado
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.ColorHex).HasMaxLength(9);
            });

            // Configuración para la gestión de usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
                entity.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(200);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Activo).IsRequired();
                
                entity.HasMany(u => u.EquiposAsignados)
                      .WithOne(e => e.Usuario)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.Nombre).IsUnique();
            });
    
            // Configuración para UsuarioRol (tabla de relación muchos a muchos)
            modelBuilder.Entity<UsuarioRol>(entity =>
            {
                entity.ToTable("UsuarioRoles");
                entity.HasKey(e => new { e.UsuarioId, e.RolId });
                
                entity.HasOne(ur => ur.Usuario)
                    .WithMany(u => u.UsuarioRoles)
                    .HasForeignKey(ur => ur.UsuarioId);
                    
                entity.HasOne(ur => ur.Rol)
                    .WithMany(r => r.UsuarioRoles)
                    .HasForeignKey(ur => ur.RolId);
            });

            modelBuilder.Entity<HistorialMovimiento>(entity =>
            {
                entity.ToTable("HistorialMovimientos");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Motivo).IsRequired().HasMaxLength(500);

                entity.HasOne(h => h.EquipoComputo)
                    .WithMany()
                    .HasForeignKey(h => h.EquipoComputoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.UsuarioAnterior)
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioAnteriorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.UsuarioNuevo)
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioNuevoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Nuevas relaciones de auditoría de ubicación
                entity.HasOne(h => h.ZonaAnterior)
                    .WithMany()
                    .HasForeignKey(h => h.ZonaAnteriorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.ZonaNueva)
                    .WithMany()
                    .HasForeignKey(h => h.ZonaNuevaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.UsuarioResponsable)
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioResponsableId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos iniciales para roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Consulta" }
            );
        }
    }
}