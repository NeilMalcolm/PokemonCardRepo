using System.Collections.Generic;

namespace PokemonCardCatalogue.Common.Models
{
    public class QueryParameters
    {
        public string OrderBy { get; set; }
        public Dictionary<string, string> Query { get; set; }
    }
}
