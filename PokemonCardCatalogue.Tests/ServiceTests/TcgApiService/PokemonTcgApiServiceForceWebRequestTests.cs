using NUnit.Framework;
using Moq;
using PokemonCardCatalogue.Common;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;

namespace PokemonCardCatalogue.Tests.ServiceTests.TcgApiService
{
    [TestFixture]
    public class PokemonTcgApiServiceForceWebRequestTests : BasePokemonTcgApiServiceTests
    {
        private const string ExampleUrl = "http://127.0.0.1/test";
        private const string CacheCardName = "This is from the cache.";
        private const string HttpClientCardName = "This is from the web.";
        
        protected override void SetupMocks()
        {
            base.SetupMocks();

            CacheMock.Setup(m => m.GetAsync<ApiResponseDataContainer<Card>>(ExampleUrl, It.IsAny<QueryParameters>()))
                .ReturnsAsync
                (
                    new ApiResponseDataContainer<Card>
                    {
                        Data = new Card
                        {
                            Name = CacheCardName
                        }
                    }
                );

            HttpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>
                (
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync
                (
                    new HttpResponseMessage
                    {
                        Content = new StringContent
                        (
                            JsonSerializer.Serialize
                            (
                                new ApiResponseDataContainer<Card>
                                {
                                    Data = new Card
                                    {
                                        Name = HttpClientCardName
                                    }
                                }
                            )
                        )
                    }
                );
        }

        [Test]
        public async Task WhenForceWebRequestIsTrue_ThenCacheIsNotAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.FetchAsync<Card>(ExampleUrl, parameters, true);

            var card = result.Data;
            Assert.IsNotNull(card);
            Assert.AreEqual(HttpClientCardName, card.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(ExampleUrl, parameters), Times.Never());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == ExampleUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task WhenForceWebRequestIsFalse_ThenCacheIsAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.FetchAsync<Card>(ExampleUrl, parameters, false);

            var card = result.Data;
            Assert.IsNotNull(card);
            Assert.AreEqual(CacheCardName, card.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(ExampleUrl, parameters), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync", 
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == ExampleUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
