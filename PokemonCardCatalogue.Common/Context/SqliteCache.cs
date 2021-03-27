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
        private readonly DateTime ReturnAllResultsDateTime = DateTime.MaxValue;
        private TimeSpan _defaultCacheDuration;
        private const string DefaultCacheFile = "ApiCache.db";

        private JsonSerializerOptions _jsonSerializerOptions;
        SQLiteAsyncConnection _sqliteAsyncConnection;

        public void Init(string filePath = null, TimeSpan? defaultCacheDuration = null)
        {
            string fileName = Path.Combine(filePath ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DefaultCacheFile);
            _defaultCacheDuration = defaultCacheDuration ?? new TimeSpan(24, 0, 0);
            
            _sqliteAsyncConnection = new SQLiteAsyncConnection(fileName ?? DefaultCacheFile);
            _sqliteAsyncConnection.CreateTableAsync<CachedQuery>();

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

                await _sqliteAsyncConnection.InsertAsync(cachedQuery);
                return;
            }

            existingValue.JsonPayload = payload;
            existingValue.LastAccessed = DateTime.UtcNow;
            existingValue.Expiry = GetExpiry(cacheDuration);

            await _sqliteAsyncConnection.UpdateAsync(existingValue);
        }

        public async Task<T> GetAsync<T>(string endpoint, QueryParameters parameters = null) where T : new()
        {
            var queryResult = await GetCachedQueryAsync(endpoint, parameters);
            var cachedItem = queryResult.Item1;

            if(cachedItem is null)
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
                await _sqliteAsyncConnection.Table<CachedQuery>()
                   .FirstOrDefaultAsync(x => x.Endpoint == endpoint
                       && x.Parameters == paramString
                       && x.Expiry > dateToCheck)
                , paramString
            );
        }

        private DateTime GetExpiry(TimeSpan? cacheDuration)
        {
            return DateTime.UtcNow.Add(cacheDuration ?? _defaultCacheDuration);
        }

        public Task ClearAllCacheAsync()
        {
            return _sqliteAsyncConnection.DeleteAllAsync<CachedQuery>();
        }
    }
}
