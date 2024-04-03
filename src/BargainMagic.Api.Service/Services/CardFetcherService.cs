using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Models;
using BargainMagic.Api.Service.Repositories;

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
        private readonly CardRepository cardRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly SeasonRepository seasonRepository;

        public CardFetcherService(CardFetcherChannel cardFetchChannel,
                                  CardRepository cardRepository,
                                  IHttpClientFactory httpClientFactory,
                                  SeasonRepository seasonRepository)
        {
            this.cardFetcherChannel = cardFetchChannel ?? throw new ArgumentNullException(nameof(cardFetchChannel));
            this.cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.seasonRepository = seasonRepository ?? throw new ArgumentNullException(nameof(seasonRepository));
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

                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                var deserializedCardModels =
                    JsonSerializer.Deserialize<List<ScryfallCardModel>>(json: cardDataJsonString,
                                                                        options: serializerOptions);

                if (deserializedCardModels == null)
                {
                    continue;
                }

                var groupedCardModels =
                    deserializedCardModels.GroupBy(c => c.Name,
                                                   c => c.Prices,
                                                   (name, prices) => new
                                                   {
                                                       Name = name,
                                                       PriceModels = prices.ToList()
                                                   })
                                          .ToList();

                var season = await seasonRepository.GetSeason(cardFetchCommmand.SeasonId);

                if (season == null)
                {
                    continue;
                }

                foreach (var cardModelGroup in groupedCardModels)
                {
                    if (cardModelGroup.Name == default ||
                        cardModelGroup.PriceModels == null ||
                        cardModelGroup.PriceModels.Count <= 0)
                    {
                        continue;
                    }

                    var cardId = await cardRepository.InsertCardAsync(cardModelGroup.Name);
                    
                    var priceList = new List<decimal>();

                    void AddPrice(string? priceString)
                    {
                        if (priceString == null)
                        {
                            return;
                        }

                        decimal.TryParse(priceString,
                                         out var decimalPrice);

                        priceList.Add(decimalPrice);
                    }

                    foreach (var priceModel in cardModelGroup.PriceModels)
                    {
                        if (priceModel == null)
                        {
                            continue;
                        }

                        AddPrice(priceModel.Usd);
                        AddPrice(priceModel.UsdFoil);
                        AddPrice(priceModel.UsdEtched);
                    }

                    if (priceList.Count <= 0)
                    {
                        continue;
                    }

                    var rawMinimumPrice = priceList.Min() * 100;

                    Console.WriteLine($"Adding Card [{cardModelGroup.Name} : {cardId}] with price [{rawMinimumPrice}]");
                    /*
                    var seasonCardComposite = new SeasonCardComposite
                                              {
                                                  Season = season,
                                                  SeasonId = season.Id,
                                                  Card = card,
                                                  CardId = card.Id,
                                                  RawCost = (int)rawMinimumPrice
                                              };
                    dataContext.SeasonCardComposites.Add(seasonCardComposite);
                    
                    await dataContext.SaveChangesAsync();
                    */
                }
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
