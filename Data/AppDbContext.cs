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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductImage> productImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar PK autogenerada
            modelBuilder.Entity<Category>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Subcategory>().Property(s => s.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductImage>().Property(ip => ip.Id).ValueGeneratedOnAdd();

            /*PRODUCT MODEL*/

            modelBuilder
                .Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(ip => ip.Product)
                .HasForeignKey(ip => ip.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            /*CATEGORY MODEL*/
            // Configurar relación uno a muchos
            modelBuilder
                .Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // opcional: eliminar subcategorías si se borra la categoría

            /*SUBCATEGORY MODEL*/
            // Relación Subcategory → Product (1:N)
            modelBuilder
                .Entity<Subcategory>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Subcategory)
                .HasForeignKey(p => p.SubcategoryId)
                .OnDelete(DeleteBehavior.Cascade); // opcional: borrar productos si borras subcategoría

            /*USER MODEL*/

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

            modelBuilder
                .Entity<User>()
                .HasOne(u => u.Contact)
                .WithOne(c => c.User)
                .HasForeignKey<Contact>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
