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
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<FeaturedProduct> FeaturedProducts { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StoreInformation> StoreInformations { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar PK autogenerada
            modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Contact>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Subcategory>().Property(s => s.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductImage>().Property(ip => ip.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>().Property(o => o.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OrderDetail>().Property(od => od.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<FeaturedProduct>().Property(f => f.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Faq>().Property(f => f.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OrderPayment>().Property(op => op.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Review>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<StoreInformation>().Property(s => s.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Location>().Property(l => l.Id).ValueGeneratedOnAdd();

            /*StoreInformation Model*/
            modelBuilder
                .Entity<StoreInformation>()
                .HasOne(s => s.Location)
                .WithMany()
                .HasForeignKey(s => s.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            /*Reviews Model*/
            modelBuilder
                .Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId);

            modelBuilder
                .Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            /*FEATURED PRODUCT MODEL*/
            modelBuilder
                .Entity<FeaturedProduct>()
                .HasOne(f => f.Product)
                .WithOne(p => p.FeaturedProduct)
                .HasForeignKey<FeaturedProduct>(f => f.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            /*ORDER DETAIL MODEL*/

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId);

            /*ORDER MODEL*/
            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder
                .Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Order>()
                .HasMany(o => o.OrderPayments)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OrderId);

            /*PRODUCT MODEL*/

            modelBuilder
                .Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(ip => ip.Product)
                .HasForeignKey(ip => ip.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

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
