using PokemonCardCatalogue.Common.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PokemonCardCatalogue.Common
{
    public static class Self
    {
        internal static string ApiKey {get;set;}

        internal static HttpClient GlobalClient { get; set; } 
            = new HttpClient();

        public static void SetCustomHttpClient(HttpClient client)
        {
            GlobalClient = client;
        }

        public static void SetApikey(string apiKey)
        {
            ApiKey = apiKey;
            SetHeaders();
        }

        private static void SetHeaders()
        {
            GlobalClient.BaseAddress = new Uri($"https://{ApiConstants.BaseUrl}");
            GlobalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            GlobalClient.DefaultRequestHeaders.Add("X-Api-key", ApiKey);
        }
    }
}
