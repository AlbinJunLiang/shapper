using AutoMapper;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shapper.Config;
using Shapper.Data;
using Shapper.Mappings;
using Shapper.Middlewares;
using Shapper.Repositories;
using Shapper.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Servicios y dependencias
// =======================
builder.Services.AddControllers();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Servicio de autenticación Firebase

// Leer ruta del JSON desde appsettings.json o variable de entorno
var firebaseJsonPath = builder.Configuration.GetValue<string>("Firebase:CredentialPath");

// o desde variable de entorno:
// var firebaseJsonPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_PATH");

// Registrar el servicio pasando la ruta
builder.Services.AddSingleton<FirebaseService>(sp => new FirebaseService(firebaseJsonPath));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("Brevo"));

// Servicio de correo
builder.Services.AddScoped<SmtpEmailStrategy>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<BrevoEmailStrategy>();

builder.Services.AddScoped<EmailStrategyFactory>();
builder.Services.AddScoped<EmailService>();

// fin de registro de servicio de correo

// Bind PayPal settings
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPal"));

// Swagger con Bearer JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shapper API", Version = "v1" });

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingresa tu token de Firebase en formato 'Bearer {token}'",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

var app = builder.Build();

// =======================
// Middlewares
// =======================

// Rutas protegidas con Firebase
app.UseWhen(
    context =>
        context.Request.Path.StartsWithSegments("/api/Users/endpoint")
        || context.Request.Path.StartsWithSegments("/api/admin")
        || context.Request.Path.StartsWithSegments("/api/privado"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<FirebaseAuthMiddleware>();
    }
);

// HTTPS
app.UseHttpsRedirection();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shapper API v1");
        c.RoutePrefix = string.Empty;
    });
}

//Se agregar el webhook antes del mapcontroller

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api/stripe/webhook"),
    appBuilder =>
    {
        appBuilder.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            }
        );
    }
);

// Autorización (HttpContext.User si FirebaseAuthMiddleware establece ClaimsPrincipal)
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

// Para los archivos del wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

//

app.Run();
