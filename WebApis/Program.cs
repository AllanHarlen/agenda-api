using AutoMapper;
using Domain;
using Domain.Interfaces;
using Domain.Services;
using Entities.Entities;
using Entities.Models;
using Infraestructure.Configuration;
using Infraestructure.Extensions;
using Infraestructure.Interfaces.Generics;
using Infraestructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebApis.Token;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development
});

// ensure env overrides are loaded
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<WebApis.Validators.ContatoModelValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Agenda API",
        Version = "v1"
    });

    // JWT Bearer auth button in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// Dynamic database configuration
void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
{
    var provider = configuration["DatabaseProvider"] ?? "SqlServer";
    var connectionString = configuration.GetConnectionString(provider);
    Console.WriteLine($"[DB] Provider: {provider}");
    Console.WriteLine($"[DB] ConnString(first 40): {connectionString?.Substring(0, Math.Min(40, connectionString.Length))}...");

    services.AddDbContext<ContextBase>(options =>
    {
        switch (provider.ToLower())
        {
            case "postgresql":
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly("Infraestructure");
                    npgsql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                });
                break;
            case "sqlserver":
            default:
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly("Infraestructure");
                    sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                });
                break;
        }

        options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        if (env.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        }
    });
}

ConfigureDatabase(builder.Services, builder.Configuration, builder.Environment);

builder.Services.AddIdentity<Usuario, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ContextBase>();

builder.Services.AddRazorPages();

// Repositories & Services
builder.Services.AddSingleton(typeof(IGeneric<>), typeof(RepositoryGenerics<>));
// Register IUsuarioRepository using factory to ensure UserManager and RoleManager are injected
builder.Services.AddScoped<IUsuarioRepository>(sp => new UsuarioRepository(
    sp.GetRequiredService<ContextBase>(),
    sp.GetRequiredService<UserManager<Usuario>>(),
    sp.GetRequiredService<RoleManager<IdentityRole>>()
));

// Contatos DI
builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
builder.Services.AddScoped<IContatoService, ContatoService>();

// Agendamentos DI
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

// Configure CORS - permissive policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Auth JWT
Settings configuracoes = new Settings();
var key = Encoding.ASCII.GetBytes(configuracoes.PrivateKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agenda API v1");
});

// Enable CORS globally
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
