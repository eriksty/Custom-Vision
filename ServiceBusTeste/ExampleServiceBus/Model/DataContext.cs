using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace ExampleServiceBus.Model
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            optionsBuilder.UseSqlServer(
                configuration["ConnectionStrings:DefaultConnection"]);
        }

        public DbSet<Teste> Teste { get; set; }
    }
}
