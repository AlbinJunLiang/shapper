using Microsoft.EntityFrameworkCore;
using Shapper.Models;

namespace Shapper.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /**
            - No puede ser nulo.
            - Debe ser como maximo 100 caracteres
            */
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();

                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder
                .Entity<User>() // Configuramos la entidad User
                .HasOne(u => u.Role) // Cada User tiene UNA propiedad de navegación hacia Role
                .WithMany(r => r.Users) // Cada Role tiene una colección de Users
                .HasForeignKey(u => u.RoleId) // La FK está en User → RoleId
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict evita que se elimine un Role si existen Users asociados.
            // Esto protege la integridad referencial y evita eliminaciones accidentales
        }
    }
}
