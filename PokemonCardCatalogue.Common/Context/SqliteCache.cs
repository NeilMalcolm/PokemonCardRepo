using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models;
using System;
using System.Threading.Tasks;
using SQLite;
using System.Text.Json;
using System.IO;
using System.Text;
using PokemonCardCatalogue.Common.Helpers;

namespace PokemonCardCatalogue.Common.Context
{
    public class SqliteCache : ICache
    {
        private readonly IDatabaseService _databaseService;

        private readonly DateTime ReturnAllResultsDateTime = DateTime.MinValue;
        private const string DefaultCacheFile = "ApiCache.db";

        private TimeSpan _cacheDuration;
        private JsonSerializerOptions _jsonSerializerOptions;

        public SqliteCache(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void Init(string filePath = null, TimeSpan? defaultCacheDuration = null)
        {
            string fileName = Path.Combine(filePath ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DefaultCacheFile);
            _cacheDuration = defaultCacheDuration ?? new TimeSpan(24, 0, 0);

            _databaseService.Init(fileName, _cacheDuration);
            _databaseService.CreateTableAsync<CachedQuery>();

            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        }

        public async Task WriteToCacheAsync(string endpoint, QueryParameters parameters, string payload, TimeSpan? cacheDuration = null)
        {
            var results = await GetCachedQueryAsync(endpoint, parameters, true);
            var existingValue = results.Item1;
            var paramString = results.Item2;

            if (existingValue is null)
            {
                var cachedQuery = new CachedQuery
                {
                    Endpoint = endpoint,
                    Parameters = paramString,
                    JsonPayload = payload,
                    Expiry = GetExpiry(cacheDuration),
                    CreatedDate = DateTime.UtcNow
                };

                await _databaseService.InsertAsync(cachedQuery);
                return;
            }

            existingValue.JsonPayload = payload;
            existingValue.LastAccessed = DateTime.UtcNow;
            existingValue.Expiry = GetExpiry(cacheDuration);

            await _databaseService.UpdateAsync(existingValue);
        }

        public async Task<T> GetAsync<T>(string endpoint, QueryParameters parameters = null) where T : new()
        {
            var queryResult = await GetCachedQueryAsync(endpoint, parameters);
            var cachedItem = queryResult.Item1;

            if(cachedItem is null)
            {
                return default;
            }

            if (string.IsNullOrWhiteSpace(cachedItem.JsonPayload))
            {
                return default;
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cachedItem.JsonPayload));
            return await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
        }

        private async Task<(CachedQuery, string)> GetCachedQueryAsync(string endpoint, QueryParameters parameters = null, bool getExpired = false)
        {
            var paramString = QueryHelper.BuildQuery(parameters);
            var orderBy = parameters?.OrderBy;
            DateTime dateToCheck = getExpired ? ReturnAllResultsDateTime : DateTime.UtcNow;
            return 
            (
                await _databaseService.FirstOrDefaultAsync<CachedQuery>(x => x.Endpoint == endpoint
                       && x.Parameters == paramString
                       && x.Expiry > dateToCheck)
                , paramString
            );
        }

        private DateTime GetExpiry(TimeSpan? cacheDuration)
        {
            return DateTime.UtcNow.Add(cacheDuration ?? _cacheDuration);
        }

        public Task ClearAllCacheAsync()
        {
            return _databaseService.DeleteAllAsync<CachedQuery>();
        }
    }
}
