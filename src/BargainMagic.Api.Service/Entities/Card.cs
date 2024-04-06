namespace BargainMagic.Api.Service.Entities
{
    public class Card
    {
        /// <summary>
        /// The unique identifier of the Card.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The human readible identifier of the Card.
        /// </summary>
        public string? Name { get; set; }
    }
}
