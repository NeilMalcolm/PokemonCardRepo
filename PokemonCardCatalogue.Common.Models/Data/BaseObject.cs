using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public abstract class BaseObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
