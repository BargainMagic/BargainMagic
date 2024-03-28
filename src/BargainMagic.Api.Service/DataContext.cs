using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var localDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localApplicationDirectoryPath = Path.Combine(localDirectoryPath, "BargainMagic");
            var applicationDatabaseDirectoryPath = Path.Combine(localApplicationDirectoryPath, "databases");
            var applicationDatabaseFilePath = Path.Combine(applicationDatabaseDirectoryPath, "BargainMagic.db");

            optionsBuilder.UseSqlite(applicationDatabaseFilePath);
        }
    }
}
