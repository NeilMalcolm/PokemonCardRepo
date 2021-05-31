using NUnit.Framework;
using Moq;
using PokemonCardCatalogue.Common;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Net.Http;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;

namespace PokemonCardCatalogue.Tests.ServiceTests.TcgApiService
{
    [TestFixture]
    public class PokemonTcgApiServiceForceWebRequestTests : BasePokemonTcgApiServiceTests
    {
        [Test]
        public async Task WhenForceWebRequestIsTrue_ThenCacheIsNotAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.FetchAsync<Card>(FetchNoCacheUrl, parameters, true);

            var card = result.Data;
            Assert.IsNotNull(card);
            Assert.AreEqual(HttpClientCardName, card.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(FetchNoCacheUrl, parameters), Times.Never());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == FetchNoCacheUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task WhenForceWebRequestIsFalse_ThenCacheIsAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.FetchAsync<Card>(FetchUrl, parameters, false);

            var card = result.Data;
            Assert.IsNotNull(card);
            Assert.AreEqual(CacheCardName, card.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(FetchUrl, parameters), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync", 
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == FetchUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
