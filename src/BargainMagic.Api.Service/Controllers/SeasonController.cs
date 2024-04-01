using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Commands;
using BargainMagic.Api.Service.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BargainMagic.Api.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> dataContextFactory;
        private readonly CardFetcherChannel cardFetcherChannel;

        public SeasonController(IDbContextFactory<DataContext> dataContextFactory,
                                CardFetcherChannel cardFetcherChannel)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            this.cardFetcherChannel = cardFetcherChannel ?? throw new ArgumentNullException(nameof(cardFetcherChannel));
        }

        [HttpPost]
        public ActionResult Create()
        {
            using var dataContext = dataContextFactory.CreateDbContext();

            var season = new Season
                         {
                             Name = "TestSeason"
                         };
            dataContext.Seasons.Add(season);

            dataContext.SaveChanges();

            var cardFetchCommand = new CardFetchCommand();
            cardFetcherChannel.Writer.TryWrite(cardFetchCommand);

            return Ok();
        }
    }
}
