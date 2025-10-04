using InventarioComputo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioComputo.Infrastructure.Persistencia
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

        public DbSet<Estado> Estados { get; set; } = null!;
        public DbSet<Unidad> Unidades { get; set; } = null!;
        public DbSet<Sede> Sedes { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Zona> Zonas { get; set; } = null!;
        public DbSet<TipoEquipo> TiposEquipo { get; set; } = null!;
        public DbSet<EquipoComputo> EquiposComputo { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<UsuarioRol> UsuarioRoles { get; set; } = null!;
        public DbSet<HistorialMovimiento> HistorialMovimientos { get; set; } = null!;
        public DbSet<BitacoraEvento> BitacoraEventos { get; set; } = null!;
        public DbSet<Empleado> Empleados { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BitacoraEvento>(entity =>
            {
                entity.ToTable("BitacoraEventos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Entidad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Detalles).HasMaxLength(1000);
            });

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
                
                // Agregar precisión a la propiedad decimal
                entity.Property(e => e.Costo)
                      .HasPrecision(18, 2);
                
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
                      
                entity.HasOne(e => e.Empleado)
                      .WithMany(emp => emp.EquiposAsignados)
                      .HasForeignKey(e => e.EmpleadoId)
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

                // Ignorar la propiedad EquiposAsignados
                entity.Ignore(u => u.EquiposAsignados);
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
                entity.HasKey(e => new { e.UsuarioId, e.RolId }); // Esto ya es correcto
    
                // No necesita una propiedad Id separada porque ya tiene una clave compuesta
    
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

                // Define claramente una sola relación con EquipoComputo
                entity.HasOne(h => h.EquipoComputo)
                    .WithMany(e => e.HistorialMovimientos) // Esta es la relación correcta
                    .HasForeignKey(h => h.EquipoComputoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // El resto de las relaciones son correctas
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

                // Relaciones con Empleado
                entity.HasOne(h => h.EmpleadoAnterior)
                    .WithMany()
                    .HasForeignKey(h => h.EmpleadoAnteriorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(h => h.EmpleadoNuevo)
                    .WithMany()
                    .HasForeignKey(h => h.EmpleadoNuevoId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // TipoEquipo: Nombre único y longitud
            modelBuilder.Entity<TipoEquipo>(entity =>
            {
                entity.ToTable("TiposEquipo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Activo).IsRequired();
            });

            // Sede: Nombre único y longitud
            modelBuilder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Activo).IsRequired();
                entity.HasMany(s => s.Areas).WithOne(a => a.Sede).HasForeignKey(a => a.SedeId);
            });

            // Area: (SedeId, Nombre) único
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Areas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
                entity.HasIndex(e => new { e.SedeId, e.Nombre }).IsUnique();
                entity.HasOne(a => a.Sede).WithMany(s => s.Areas).HasForeignKey(a => a.SedeId);
                entity.HasMany(a => a.Zonas).WithOne(z => z.Area).HasForeignKey(z => z.AreaId);
            });

            // Zona: (AreaId, Nombre) único
            modelBuilder.Entity<Zona>(entity =>
            {
                entity.ToTable("Zonas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
                entity.HasIndex(e => new { e.AreaId, e.Nombre }).IsUnique();
                entity.HasOne(z => z.Area).WithMany(a => a.Zonas).HasForeignKey(z => z.AreaId);
            });

            // Datos iniciales para roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administradores" },
                new Rol { Id = 2, Nombre = "Soporte" },
                new Rol { Id = 3, Nombre = "Consulta" }
            );
        }
    }
}