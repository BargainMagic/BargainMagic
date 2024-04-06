using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Commands;
using BargainMagic.Api.Service.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace BargainMagic.Api.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController : ControllerBase
    {
        private readonly CardFetcherChannel cardFetcherChannel;
        private readonly SeasonRepository seasonRepository;

        public SeasonController(CardFetcherChannel cardFetcherChannel,
                                SeasonRepository seasonRepository)
        {
            this.cardFetcherChannel = cardFetcherChannel ?? throw new ArgumentNullException(nameof(cardFetcherChannel));
            this.seasonRepository = seasonRepository ?? throw new ArgumentNullException(nameof(seasonRepository));
        }

        [HttpPost]
        public async Task<ActionResult> Create(string seasonName)
        {
            var seasonId = await seasonRepository.InsertSeason(seasonName);

            var cardFetchCommand = new CardFetchCommand
                                   {
                                       SeasonId = seasonId
                                   };

            cardFetcherChannel.Writer.TryWrite(cardFetchCommand);

            return Ok();
        }
    }
}
