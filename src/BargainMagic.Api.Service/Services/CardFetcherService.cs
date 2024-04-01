using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Models;

using System.Net.Http;
using System.Text.Json;

namespace BargainMagic.Api.Service.Services
{
    public class CardFetcherService : BackgroundService
    {
        #region Constants

        protected const string DefaultCardsEndpointInformationUri = "https://api.scryfall.com/bulk-data/default-cards";
        protected const string HttpClientName = "Scryfall";

        #endregion Constants

        private readonly CardFetcherChannel cardFetcherChannel;
        private readonly IHttpClientFactory httpClientFactory;

        public CardFetcherService(CardFetcherChannel cardFetchChannel,
                                  IHttpClientFactory httpClientFactory)
        {
            this.cardFetcherChannel = cardFetchChannel ?? throw new ArgumentNullException(nameof(cardFetchChannel));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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

                var cardDataJsonString = await GetCardDataAsync();
            }
        }

        public async Task<string> GetCardDataAsync()
        {
            using var httpClient = this.httpClientFactory.CreateClient(HttpClientName);
            
            var endpointInformation = await this.GetDefaultCardEndpointInformationAsync(httpClient);

            if (endpointInformation == null)
            {
                throw new Exception("Failed retrieving endpoint information from Scryfall.");
            }

            if (string.IsNullOrWhiteSpace(endpointInformation.DownloadUri))
            {
                throw new Exception("Retrieved Skryfall endpoint information did not contain a download URI.");
            }

            return await httpClient.GetStringAsync(endpointInformation.DownloadUri);
        }

        private async Task<BulkDataEndpointResponse?> GetDefaultCardEndpointInformationAsync(HttpClient httpClient)
        {
            var endpointInformationString = await httpClient.GetStringAsync(DefaultCardsEndpointInformationUri);

            try
            {
                return JsonSerializer.Deserialize<BulkDataEndpointResponse>(endpointInformationString);
            }
            catch (Exception)
            {
                // TODO: Add logging here.
            }

            return null;
        }
    }
}
