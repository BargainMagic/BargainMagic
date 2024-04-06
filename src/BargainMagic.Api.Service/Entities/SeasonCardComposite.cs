namespace BargainMagic.Api.Service.Entities
{
    public class SeasonCardComposite
    {
        /// <summary>
        /// The unique identifier of the Season.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// The unique identifier of the Card.
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// The "raw" cost value of the card. This is a whole number representation of the associated USD cost and will
        /// need to be formatted for display purposes.
        /// </summary>
        public int RawCost { get; set; }

        /// <summary>
        /// The Season entity associated with the <see cref="SeasonId"/>.
        /// </summary>
        public Season? Season { get; set; }

        /// <summary>
        /// The Card entity associated with the <see cref="CardId"/>.
        /// </summary>
        public Card? Card { get; set; }
    }
}
