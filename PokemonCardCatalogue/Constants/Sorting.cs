using PokemonCardCatalogue.Enums;
using System.Collections.Generic;

namespace PokemonCardCatalogue.Constants
{
    public static class Sorting
    {
        public static List<KeyValuePair<SortOrder, string>> SortModes = 
            new List<KeyValuePair<SortOrder, string>>
        {
            new KeyValuePair<SortOrder, string>(SortOrder.NumericAscending, "Number Asc"),
            new KeyValuePair<SortOrder, string>(SortOrder.NumericDescending, "Number Desc"),
            new KeyValuePair<SortOrder, string>(SortOrder.Rarity, "Rarity")
        };
    }
}
