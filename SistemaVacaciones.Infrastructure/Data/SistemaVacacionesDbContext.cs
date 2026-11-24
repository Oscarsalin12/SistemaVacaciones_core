using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Infrastructure.Data
{
    public class SistemaVacacionesDbContext : DbContext
    {
        public SistemaVacacionesDbContext(DbContextOptions<SistemaVacacionesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Departamento> Departamentos => Set<Departamento>();
        public DbSet<Empresa> Empresas => Set<Empresa>();
        public DbSet<SubEmpresa> SubEmpresas => Set<SubEmpresa>();
        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<MotivoAusencia> MotivosAusencia => Set<MotivoAusencia>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<UsuarioEmpresa> UsuariosEmpresas => Set<UsuarioEmpresa>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Departamentos
            modelBuilder.Entity<Departamento>(entity =>
            {
                entity.ToTable("Departamentos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClaveDep).HasMaxLength(50);
                entity.Property(e => e.NombreDep).HasMaxLength(100);
                entity.Property(e => e.NombreEmcargado).HasMaxLength(150);
            });

            // Empresas
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Rfc).HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(200);
            });

            // SubEmpresas
            modelBuilder.Entity<SubEmpresa>(entity =>
            {
                entity.ToTable("SubEmpresas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Rfc).HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Sucursal).HasMaxLength(100);

                entity.HasOne(s => s.Empresa)
                    .WithMany(e => e.SubEmpresas)
                    .HasForeignKey(s => s.IdEmpresa_madre)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Empleados
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleados");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.UsuarioEmpleado).HasMaxLength(100);
                entity.Property(e => e.Estado).HasMaxLength(50);

                entity.HasOne(e => e.Departamento)
                    .WithMany(d => d.Empleados)
                    .HasForeignKey(e => e.IdDepartamento)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SubEmpresa)
                    .WithMany(s => s.Empleados)
                    .HasForeignKey(e => e.IdEmpresa)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MotivosAusencia
            modelBuilder.Entity<MotivoAusencia>(entity =>
            {
                entity.ToTable("MotivosAusencia");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreMotivo).HasMaxLength(100).IsRequired();
            });

            // Permisos
            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.ToTable("Permisos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Estado).HasMaxLength(50);

                entity.HasOne(p => p.Empleado)
                    .WithMany(e => e.Permisos)
                    .HasForeignKey(p => p.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.MotivoAusencia)
                    .WithMany(m => m.Permisos)
                    .HasForeignKey(p => p.IdTipoPermiso)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SubEmpresa)
                    .WithMany(s => s.Permisos)
                    .HasForeignKey(p => p.IdEmpresa)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Roles
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            });

            // Usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NombreUsuario).HasColumnName("Usuario").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Contrasenia).HasMaxLength(200).IsRequired();
            });

            // UsuariosEmpresas
            modelBuilder.Entity<UsuarioEmpresa>(entity =>
            {
                entity.ToTable("UsuariosEmpresas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
                entity.Property(e => e.IdEmpresa).HasColumnName("idEmpresa");
                entity.Property(e => e.IdRol).HasColumnName("idRol");
                entity.Property(e => e.IdSubEmpresa).HasColumnName("idSubEmpresa");

                entity.HasOne(ue => ue.Usuario)
                    .WithMany(u => u.UsuariosEmpresas)
                    .HasForeignKey(ue => ue.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ue => ue.Empresa)
                    .WithMany(e => e.UsuariosEmpresas)
                    .HasForeignKey(ue => ue.IdEmpresa)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ue => ue.Rol)
                    .WithMany(r => r.UsuariosEmpresas)
                    .HasForeignKey(ue => ue.IdRol)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ue => ue.SubEmpresa)
                    .WithMany(s => s.UsuariosEmpresas)
                    .HasForeignKey(ue => ue.IdSubEmpresa)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
