using PokemonCardCatalogue.Common.Models.Data;
using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common
{
    public class ApiListResponseDataContainer<T> where T : BaseObject
    {
        [JsonPropertyName("data")]
        public T[] Data { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
