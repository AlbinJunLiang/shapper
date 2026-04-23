using Shapper.Repositories.Categories;
using Shapper.Repositories.Faqs;
using Shapper.Repositories.FeaturedProducts;
using Shapper.Repositories.Locations;
using Shapper.Repositories.OrderDetails;
using Shapper.Repositories.OrderPayments;
using Shapper.Repositories.Orders;
using Shapper.Repositories.Products;
using Shapper.Repositories.Reviews;
using Shapper.Repositories.Roles;
using Shapper.Repositories.Stores;
using Shapper.Repositories.StoreLinks;
using Shapper.Repositories.Subcategories;
using Shapper.Repositories.Users;
using Shapper.Services.Categories;
using Shapper.Services.Checkouts;
using Shapper.Services.Faqs;
using Shapper.Services.FeaturedProducts;
using Shapper.Services.Locations;
using Shapper.Services.OrderDetails;
using Shapper.Services.OrderPayments;
using Shapper.Services.Orders;
using Shapper.Services.PaymentWebhooks;
using Shapper.Services.Products;
using Shapper.Services.Reviews;
using Shapper.Services.Roles;
using Shapper.Services.Stores;
using Shapper.Services.StoreLinks;
using Shapper.Services.Subcategories;
using Shapper.Services.Users;
using Shapper.Repositories.ProductImages;
using Shapper.Services.ProductImages;

namespace Shapper.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IFaqRepository, FaqRepository>();
            services.AddScoped<IFeaturedProductRepository, FeaturedProductRepository>();
            services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubcategoryService, SubcategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IFeaturedProductService, FeaturedProductService>();
            services.AddScoped<IOrderPaymentService, OrderPaymentService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ILocationService, LocationService>();

            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IPaymentWebhookService, PaymentWebhookService>();

            // Agrega estas líneas en el método AddApplicationServices

            // En el método AddApplicationServices, agrega:
            services.AddScoped<IStoreLinkRepository, StoreLinkRepository>();
            services.AddScoped<IStoreLinkService, StoreLinkService>();
            services.AddScoped<IProductImageService, ProductImageService>();

            return services;
        }
    }
}
