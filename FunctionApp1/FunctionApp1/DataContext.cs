using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("local.settings.json");
            //var configuration = builder.Build();

            optionsBuilder.UseSqlServer("Server=DESKTOP-2ME9TD4\\\\SQLEXPRESS;Database=Admin;Trusted_Connection=True");

            //optionsBuilder.UseSqlServer(
            //    configuration["ConnectionStrings:DefaultConnection"]);
        }

        public DbSet<Prediction> Predictions { get; set; }
    }
}
