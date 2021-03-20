using System;
using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Set : BaseObject
    {
        [JsonPropertyName("series")]
        public string Series { get; set; }

        [JsonPropertyName("printedTotal")]
        public int PrintedTotal { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("legalities")]
        public Legalities Legalities { get; set; }

        [JsonPropertyName("ptcgoCode")]
        public string PokemonTcgOnlineCode { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("images")]
        public SetImages Images { get; set; }

        [JsonPropertyName("_self")]
        public string Self { get; set; }
    }
}
