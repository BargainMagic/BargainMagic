using BargainMagic.Api.Service.Channels;

namespace BargainMagic.Api.Service.Services
{
    public class CardFetcherService : BackgroundService
    {
        private readonly CardFetcherChannel cardFetcherChannel;

        public CardFetcherService(CardFetcherChannel cardFetchChannel)
        {
            this.cardFetcherChannel = cardFetchChannel ?? throw new ArgumentNullException(nameof(cardFetchChannel));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await cardFetcherChannel.Reader.WaitToReadAsync(cancellationToken);

                if (!cardFetcherChannel.Reader.TryRead(out var cardFetchCommmand))
                {
                    continue;
                }

                Console.WriteLine(cardFetchCommmand.ToString());
            }
        }
    }
}
