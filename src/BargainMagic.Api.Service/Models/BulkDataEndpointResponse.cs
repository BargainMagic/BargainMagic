using System.Text.Json.Serialization;

namespace BargainMagic.Api.Service.Models
{
    public class BulkDataEndpointResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("size")]
        public uint? Size { get; set; }

        [JsonPropertyName("download_uri")]
        public string? DownloadUri { get; set; }

        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("content_encoding")]
        public string? ContentEncoding { get; set; }
    }
}
