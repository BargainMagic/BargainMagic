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
    }
}
