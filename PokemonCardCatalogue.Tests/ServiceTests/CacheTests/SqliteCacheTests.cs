using Moq;
using NUnit.Framework;
using PokemonCardCatalogue.Common;
using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Helpers;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ServiceTests.CacheTests
{
    [TestFixture]
    public class SqliteCacheTests : BaseTestFixture
    {
        private const string ValidCachedRequestUrl = "http://127.0.0.1/cached";
        private QueryParameters ValidCachedParameters = new QueryParameters
        {
            Query = new Dictionary<string, string>
            {
                { "Valid", "Request" }
            }
        };
        private const string ExpiredCachedRequestUrl = "http://127.0.0.1/expired";
        private QueryParameters ExpiredCachedParameters = new QueryParameters
        {
            Query = new Dictionary<string, string>
            {
                { "Expired", "Request" }
            }
        };

        private List<CachedQuery> CachedQueryData;

        protected DbCache Cache;
        protected Mock<IDatabaseService> DatabaseServiceMock;

        protected override void SetupMocks()
        {
            base.SetupMocks();

            DatabaseServiceMock = new Mock<IDatabaseService>();

            DatabaseServiceMock.Setup(m => m.InsertAsync(It.IsAny<CachedQuery>()))
                .Callback<CachedQuery>(itemToInsert => CachedQueryData.Add(itemToInsert));

            DatabaseServiceMock.Setup(m => m.UpdateAsync(It.IsAny<CachedQuery>()))
                .Callback<CachedQuery>(itemToInsert =>
                {
                    var existingItem = CachedQueryData.FirstOrDefault(m => m.Endpoint == itemToInsert.Endpoint
                        && m.Parameters == itemToInsert.Parameters);

                    var index = CachedQueryData.IndexOf(existingItem);

                    CachedQueryData.RemoveAt(index);
                    CachedQueryData.Insert(index, itemToInsert);
                });


            DatabaseServiceMock.Setup(m => m.FirstOrDefaultAsync<CachedQuery>(It.IsAny<Expression<Func<CachedQuery, bool>>>()))
                .Returns<Expression<Func<CachedQuery, bool>>>
                ((prexExpr) =>
                {
                    return Task.FromResult
                    (
                        CachedQueryData.FirstOrDefault(prexExpr.Compile())
                    );
                });

            Cache = new DbCache
            (
                DatabaseServiceMock.Object
            );
        }

        protected override void SetupData()
        {
            base.SetupData();
            CachedQueryData = new List<CachedQuery>
            {
                new CachedQuery
                {
                    Endpoint = ValidCachedRequestUrl,
                    Parameters = QueryHelper.BuildQuery(ValidCachedParameters),
                    Expiry = DateTime.UtcNow.AddDays(2),
                    JsonPayload =
                    @"{""data"":{""supertype"":null,""subtypes"":null,""hp"":null,""types"":null,""evolvesTo"":null,""attacks"":null,""weaknesses"":null,""retreatCost"":null,""convertedRetreatCost"":0,""set"":null,""number"":null,""artist"":null,""rarity"":null,""nationalPokedexNumbers"":null,""legalities"":null,""images"":null,""tcgplayer"":null,""id"":null,""name"":""Example""},""page"":0,""pageSize"":0,""count"":0,""totalCount"":0}"
                },
                new CachedQuery
                {
                    Endpoint = ExpiredCachedRequestUrl,
                    Parameters = QueryHelper.BuildQuery(ExpiredCachedParameters),
                    Expiry = DateTime.UtcNow.AddDays(-5)
                }
            };
        }

        [Test]
        public async Task WhenThereIsNoValueInCache_ThenGetAsyncReturnsNull()
        {
            var result = await Cache.GetAsync<ApiResponseDataContainer<Card>>
            (
                "http://127.0.0.1/NoCacheUrl"
            );

            Assert.IsNull(result);
        }

        [Test]
        public async Task WhenThereIsAnExpiredValueInCache_ThenGetAsyncReturnsNull()
        {
            var result = await Cache.GetAsync<ApiResponseDataContainer<Card>>
            (
                ExpiredCachedRequestUrl,
                ExpiredCachedParameters
            );

            Assert.IsNull(result);
        }

        [Test]
        public async Task WhenThereIsAnNonExpiredValueInCache_ThenGetAsyncReturnsObjectInCache()
        {
            var result = await Cache.GetAsync<ApiResponseDataContainer<Card>>
            (
                ValidCachedRequestUrl,
                ValidCachedParameters
            );
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task WhenWriteToCacheAsyncIsCalled_AndThereIsNoExistingValue_ThenInsertsValue()
        {
            var newCacheItemEndpoint = "http://127.0.0.1/NewItemToCache";
            var newCacheItemPayload = "payload";
            await Cache.WriteToCacheAsync
            (
                newCacheItemEndpoint,
                new QueryParameters(),
                newCacheItemPayload
            );

            Assert.IsTrue(CachedQueryData.Any(m => m.Endpoint == newCacheItemEndpoint));
            DatabaseServiceMock.Verify
            (
                m => m.InsertAsync(It.Is<CachedQuery>
                (
                    q => q.Endpoint == newCacheItemEndpoint
                    && q.JsonPayload == newCacheItemPayload
                )),
                Times.Once()
            );
        }

        [Test]
        public async Task WhenWriteToCacheAsyncIsCalled_AndThereIsPreExistingValue_ThenUpdatesValue()
        {
            var existingCacheItemPayload = "payload";
            await Cache.WriteToCacheAsync
            (
                ExpiredCachedRequestUrl,
                ExpiredCachedParameters,
                existingCacheItemPayload
            );

            Assert.IsTrue(CachedQueryData.Any(m => m.Endpoint == ExpiredCachedRequestUrl && m.JsonPayload == existingCacheItemPayload));
            DatabaseServiceMock.Verify
            (
                m => m.UpdateAsync(It.Is<CachedQuery>
                (
                    q => q.Endpoint == ExpiredCachedRequestUrl
                    && q.Parameters == QueryHelper.BuildQuery(ExpiredCachedParameters)
                    && q.JsonPayload == existingCacheItemPayload
                )),
                Times.Once()
            );
        }
    }
}
