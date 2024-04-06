using BargainMagic.Api.Service.Entities;

using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service.Repositories
{
    public class CardRepository
    {
        private readonly IDbContextFactory<DataContext> dataContextFactory;

        public CardRepository(IDbContextFactory<DataContext> dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        public async Task<int> InsertCardAsync(string cardName)
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            // Check to see if the card 
            var card = dataContext.Cards.FirstOrDefault(c => c.Name == cardName);

            if (card == null)
            {
                card = new Card
                {
                    Name = cardName
                };

                dataContext.Cards.Add(card);
            }

            await dataContext.SaveChangesAsync();

            return card.Id;
        }
    
        public async Task InsertSeasonCardCompositeAsync(int seasonId,
                                                         int cardId,
                                                         int rawCost)
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            var seasonCardComposite = dataContext.SeasonCardComposites.FirstOrDefault(scc => scc.SeasonId == seasonId
                                                                                          && scc.CardId == cardId);

            if (seasonCardComposite != null)
            {
                return;
            }

            seasonCardComposite = new SeasonCardComposite
                                  {
                                      SeasonId = seasonId,
                                      CardId = cardId,
                                      RawCost = rawCost
                                  };

            dataContext.SeasonCardComposites.Add(seasonCardComposite);

            await dataContext.SaveChangesAsync();
        }
    }
}
