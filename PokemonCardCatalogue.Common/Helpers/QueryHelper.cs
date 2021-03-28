using PokemonCardCatalogue.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonCardCatalogue.Common.Helpers
{
    public static class QueryHelper
    {
        private const string QueryParameter = "?q=";

        public static string BuildQuery(QueryParameters parameters)
        {
            if (parameters is null)
            {
                return string.Empty;
            }

            var queryParameters = parameters.Query;

            StringBuilder sb = new StringBuilder();

            if (queryParameters != null && queryParameters.Count > 0)
            {
                sb.Append(QueryParameter);

                for (int i = 0; i < queryParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(' ');
                    }

                    var kvp = queryParameters.ElementAt(i);
                    sb.Append(kvp.Key);
                    sb.Append(':');
                    sb.Append(kvp.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                sb.Append(sb.Length > 0 ? '&' : '?');
                sb.Append("orderBy=");
                sb.Append(parameters.OrderBy);
            }

            return sb.ToString();
        }

        public static List<KeyValuePair<string, string>> GetPokedexNumberQuery(int[] pokedexNumbers)
        {
            if (pokedexNumbers is null)
            {
                return new List<KeyValuePair<string, string>>();
            }

            var numberQuery = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < pokedexNumbers.Length; i++)
            {
                var number = pokedexNumbers[i];
                string key = "(nationalPokedexNumbers";
                string value;

                if (pokedexNumbers.Length != 1 && i < pokedexNumbers.Length - 1)
                {
                    value = $"[{number} TO {number}]) or";
                }
                else
                {
                    value = $"[{number} TO {number}])";
                }

                numberQuery.Add(new KeyValuePair<string, string>(key, value));
            }

            return numberQuery;
        }

        public static string GetCardBaseName(string cardName)
        {
            return $"*{cardName.Split(' ')[0]}*";
        }
    }
}
