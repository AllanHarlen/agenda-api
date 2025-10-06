using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infraestructure.Configuration
{
    public class ContextBaseFactory : IDesignTimeDbContextFactory<ContextBase>
    {
        public ContextBase CreateDbContext(string[] args)
        {
            // Resolve a base path que contenha appsettings.json (raiz ou WebApis)
            var cwd = Directory.GetCurrentDirectory();
            var webApisPath = Path.Combine(cwd, "WebApis");
            var basePath = File.Exists(Path.Combine(cwd, "appsettings.json")) ? cwd :
                           (File.Exists(Path.Combine(webApisPath, "appsettings.json")) ? webApisPath : cwd);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables() // permite override via ConnectionStrings__*
                .Build();

            var provider = configuration["DatabaseProvider"] ?? "SqlServer";
            var connectionString = configuration.GetConnectionString(provider);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"ConnectionStrings:{provider} não encontrada na configuração.");

            var optionsBuilder = new DbContextOptionsBuilder<ContextBase>();
            switch (provider.ToLowerInvariant())
            {
                case "postgresql":
                    optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Infraestructure"));
                    break;
                default:
                    optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infraestructure"));
                    break;
            }

            return new ContextBase(optionsBuilder.Options);
        }
    }
}
