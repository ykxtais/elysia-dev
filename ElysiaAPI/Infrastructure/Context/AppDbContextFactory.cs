using System.IO;
using ElysiaAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// Db context para criar o AppDbContext sem passar pelo Program.cs,
// então a falta da chave jwt não atrapalha as migrations, já que vamos utilizar só moto e vaga na api!

namespace ElysiaAPI.Infrastructure.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
           
            var basePath = Directory.GetCurrentDirectory();
            
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = config.GetConnectionString("Postgres")
                     ?? "Host=localhost;Port=5432;Database=elysia;Username=elysiaadmin;Password=dev;";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(cs);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}