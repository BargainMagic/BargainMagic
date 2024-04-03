using System.Text.Json.Serialization;

namespace BargainMagic.Api.Service.Models
{
    public class ScryfallCardModel
    {
        public string? Name { get; set; }
        public ScryfallPricesModel? Prices { get; set; }
    }
}
