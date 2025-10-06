using Infraestructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infraestructure.DesignTime
{
    public class ContextFactory : IDesignTimeDbContextFactory<ContextBase>
    {
        public ContextBase CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Determine solution root (Infraestructure is sibling to WebApis)
            var current = Directory.GetCurrentDirectory();
            var root = current;
            if (Directory.Exists(Path.Combine(current, "..", "WebApis")))
            {
                root = Path.GetFullPath(Path.Combine(current, ".."));
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(root, "WebApis"))
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var provider = (config["DatabaseProvider"] ?? "SqlServer").ToLowerInvariant();
            var sqlServer = config.GetConnectionString("SqlServer");
            if (string.IsNullOrWhiteSpace(sqlServer))
                throw new InvalidOperationException("ConnectionStrings:SqlServer não configurada no appsettings.");

            var builder = new DbContextOptionsBuilder<ContextBase>();
            if (provider == "postgresql")
            {
                builder.UseNpgsql(sqlServer, x => x.MigrationsAssembly("Infraestructure"));
            }
            else
            {
                builder.UseSqlServer(sqlServer, x => x.MigrationsAssembly("Infraestructure"));
            }

            return new ContextBase(builder.Options);
        }
    }
}
