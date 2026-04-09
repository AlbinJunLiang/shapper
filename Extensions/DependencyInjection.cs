using Microsoft.Extensions.DependencyInjection;
using Shapper.Repositories.Categories;
using Shapper.Repositories.Faqs;
using Shapper.Repositories.FeaturedProducts;
using Shapper.Repositories.Locations;
using Shapper.Repositories.OrderDetails;
using Shapper.Repositories.OrderPayments;
using Shapper.Repositories.Products;
using Shapper.Repositories.Reviews;
using Shapper.Repositories.Roles;
using Shapper.Repositories.StoreInformations;
using Shapper.Repositories.Subcategories;
using Shapper.Repositories.Users;
using Shapper.Services.Categories;
using Shapper.Services.Faqs;
using Shapper.Services.FeaturedProducts;
using Shapper.Services.Locations;
using Shapper.Services.OrderDetails;
using Shapper.Services.OrderPayments;
using Shapper.Services.Products;
using Shapper.Services.Reviews;
using Shapper.Services.Roles;
using Shapper.Services.StoreInformations;
using Shapper.Services.Subcategories;
using Shapper.Services.Users;

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
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IStoreInformationRepository, StoreInformationRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();

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
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IStoreInformationService, StoreInformationService>();
            services.AddScoped<ILocationService, LocationService>();

            return services;
        }
    }
}
