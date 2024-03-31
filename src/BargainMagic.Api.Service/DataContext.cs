using BargainMagic.Api.Service.Entities;

using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service
{
    public class DataContext : DbContext
    {
        public DbSet<Season> Seasons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var localDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localApplicationDirectoryPath = Path.Combine(localDirectoryPath, "BargainMagic");
            var applicationDatabaseDirectoryPath = Path.Combine(localApplicationDirectoryPath, "databases");
            var applicationDatabaseFilePath = Path.Combine(applicationDatabaseDirectoryPath, "BargainMagic.db");

            var connectionString = string.Format("Data Source={0};",
                                                 applicationDatabaseFilePath);

            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Season>().ToTable("Season");
        }
    }
}
