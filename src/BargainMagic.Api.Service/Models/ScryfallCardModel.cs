namespace BargainMagic.Api.Service.Models
{
    public class ScryfallCardModel
    {
        public string? Name { get; set; }
        public string? Layout { get; set; }
        public string? TypeLine { get; set; }
        public List<ScryfallCardFaceModel>? CardFaces { get; set; }
        public ScryfallPricesModel? Prices { get; set; }
    }
}
