﻿using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Models;
using BargainMagic.Api.Service.Repositories;

using System.Text.Json;
using System.Text.RegularExpressions;

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

                const string NotLegal = "not_legal";
                const string ReversableCardLayout = "reversible_card";
                const string TypeLineRegex = "Token|Basic";

                var filteredCardModels = new List<ScryfallCardModel>();
                
                foreach (var cardModel in deserializedCardModels)
                {
                    if (cardModel.Layout == ReversableCardLayout &&
                        cardModel.CardFaces != null)
                    {
                        var frontCardFace = cardModel.CardFaces.FirstOrDefault();

                        if (frontCardFace != null)
                        {
                            cardModel.Name = frontCardFace.Name;
                            cardModel.TypeLine = frontCardFace.TypeLine;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (cardModel.TypeLine == null ||
                        Regex.Match(cardModel.TypeLine, TypeLineRegex).Success)
                    {
                        continue;
                    }

                    if (cardModel.Legalities == null ||
                        cardModel.Legalities.All(l => l.Value == NotLegal))
                    {
                        continue;
                    }
                    
                    filteredCardModels.Add(cardModel);
                }

                var groupedCardModels =
                    filteredCardModels.GroupBy(c => c.Name,
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

                    await cardRepository.InsertSeasonCardCompositeAsync(seasonId: season.Id,
                                                                        cardId: cardId,
                                                                        rawCost: (int)rawMinimumPrice);
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
