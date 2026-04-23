using System.Security.Claims;
using System.Text;
using AutoMapper;
using CloudinaryDotNet;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shapper.Authentication;
using Shapper.Config;
using Shapper.Data;
using Shapper.Extensions;
using Shapper.Mappings;
using Shapper.Services;
using Shapper.Services.Emails;
using Shapper.Services.Emails.Strategies;
using Shapper.Services.Firebase;
using Shapper.Services.ImageStorage;
using Shapper.Services.ImageStorage.Strategies;
using Shapper.Services.Payment;
using Shapper.Services.Payment.Strategies;
using Shapper.Services.PaymentUrlValidators;
using Shapper.Services.Verifications;
using Shapper.Services.Verifications.Strategies;
using Stripe;

// dotnet run --environment Development

var builder = WebApplication.CreateBuilder(args);
var status = builder.Environment.IsDevelopment();

// =======================
// Servicios y dependencias
// =======================
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddApplicationServices();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Servicio de autenticación Firebase

// 1. Intentamos leer la ruta
var firebaseJsonPath =
    builder.Configuration.GetValue<string>("Firebase:CredentialPath")
    ?? throw new InvalidOperationException("Firebase CredentialPath is missing in configuration.");

// 2. Registrar el servicio de forma segura
builder.Services.AddSingleton<FirebaseService>(sp => new FirebaseService(firebaseJsonPath));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("Brevo"));

builder.Services.Configure<UrlSettings>(builder.Configuration.GetSection(UrlSettings.SectionName));

// También puedes registrar tu validador como Singleton
builder.Services.AddSingleton<IPaymentUrlValidator, PaymentUrlValidator>();

// Servicio de correo
builder.Services.AddScoped<SmtpEmailStrategy>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<BrevoEmailStrategy>();

builder.Services.AddScoped<EmailStrategyFactory>();
builder.Services.AddScoped<EmailService>();

// 1. Registrar el servicio base de Firebase

// 2. Registrar la estrategia (necesaria para que la fábrica la encuentre)
// builder.Services.AddScoped<SupabaseVerificationStrategy>();

builder.Services.AddScoped<FirebaseVerificationStrategy>();

// 3. Registrar la fábrica
builder.Services.AddScoped<VerificationStrategyFactory>();

// fin de registro de servicio de correo

// Bind PayPal settings
builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPal"));

builder.Services.AddScoped<IPaymentStrategy, PaypalPaymentStrategy>();
builder.Services.AddScoped<IPaymentStrategy, StripePaymentStrategy>();

builder.Services.AddScoped<PaymentService>();

builder.Services.AddSingleton(provider =>
{
    var url = builder.Configuration["Cloudinary:ImageStorageUrl"];

    return new CloudinaryDotNet.Cloudinary(url);
});


// Estrategias de imágenes
builder.Services.AddScoped<LocalImageStrategy>();
builder.Services.AddScoped<CloudinaryImageStrategy>();
builder.Services.AddScoped<ImageStrategyFactory>();

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

// 1. Definir la política
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Provider"; // <--- Aquí
        options.DefaultChallengeScheme = "Provider"; // <--- Aquí
    })
    .AddScheme<AuthenticationSchemeOptions, ProviderAuthenticationHandler>("Provider", null);

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailVerifiedOnly", policy => policy.RequireClaim("email_verified", "true"));
    options.AddPolicy(
        "AdminOnly",
        policy =>
        {
            policy.RequireClaim(ClaimTypes.Email, "liangalbin9@gmail.com");
        }
    );
});

var app = builder.Build();

// 2. Usar la política (IMPORTANTE: Antes de UseAuthorization)
app.UseCors("AllowAll");

// =======================
// Middlewares
// =======================

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
app.UseAuthentication();
app.UseAuthorization();


// Mapear controllers
app.MapControllers();

// Para los archivos del wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

//

app.Run();

// Permitir las pruebas
public partial class Program { }
