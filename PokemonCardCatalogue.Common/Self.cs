using PokemonCardCatalogue.Common.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PokemonCardCatalogue.Common
{
    public static class Self
    {
        internal static string ApiKey {get;set;}

        internal static string BaseAddress { get; set; }
        internal static HttpClient GlobalHttpClient { get; set; }
            = CreateDefaultHttpClient();

        public static void SetCustomHttpClient(HttpClient customHttpClient)
        {
            GlobalHttpClient = customHttpClient;
        }

        public static void SetApikey(string apiKey)
        {
            ApiKey = apiKey;
            SetHeaders(GlobalHttpClient);
        }

        internal static void SetHeaders(HttpClient client)
        {
            client.BaseAddress = new Uri($"https://{ApiConstants.BaseUrl}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-key", ApiKey);
        }

        private static HttpClient CreateDefaultHttpClient()
        {
            var httpclient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            return httpclient;
        }
    }
}
