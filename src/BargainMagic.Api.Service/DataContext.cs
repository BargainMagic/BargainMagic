using BargainMagic.Api.Service.Entities;

using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service
{
    public class DataContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<SeasonCardComposite> SeasonCardComposites { get; set; }

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
            modelBuilder.Entity<Card>()
                        .ToTable("Card")
                        .HasKey(e => e.Id);

            modelBuilder.Entity<Season>()
                        .ToTable("Season")
                        .HasKey(e => e.Id);

            modelBuilder.Entity<SeasonCardComposite>(entity =>
            {
                entity.ToTable("SeasonCardComposite");
                entity.HasKey(e => new { e.SeasonId, e.CardId });

                entity.HasOne(e => e.Season)
                      .WithMany()
                      .HasForeignKey(e => e.SeasonId);

                entity.HasOne(e => e.Card)
                      .WithMany()
                      .HasForeignKey(e => e.CardId);
            });
        }
    }
}
