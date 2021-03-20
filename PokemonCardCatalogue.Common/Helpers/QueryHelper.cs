using PokemonCardCatalogue.Common.Models;
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
    }
}
