using BargainMagic.Api.Service.Entities;

using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service.Repositories
{
    public class SeasonRepository
    {
        private readonly IDbContextFactory<DataContext> dataContextFactory;

        public SeasonRepository(IDbContextFactory<DataContext> dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        public async Task<Season?> GetSeason(int seasonId)
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            return await dataContext.Seasons.FirstOrDefaultAsync(s => s.Id == seasonId);
        }

        public async Task<int> InsertSeason(string seasonName)
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            var season = dataContext.Seasons.FirstOrDefault(s => s.Name == seasonName);

            if (season != null)
            {
                return default;
            }

            season = new Season
                     {
                         Name = seasonName
                     };

            dataContext.Seasons.Add(season);

            await dataContext.SaveChangesAsync();

            return season.Id;

        }
    }
}
