using Moq;
using Moq.Protected;
using NUnit.Framework;
using PokemonCardCatalogue.Common;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ServiceTests.TcgApiService
{
    [TestFixture]
    public class PokemonTcgApiServiceCacheTests : BasePokemonTcgApiServiceTests
    {
        [Test]
        public async Task WhenFetchAsyncIsCalled_AndValueExistsInCache_ThenWebIsNotAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.FetchAsync<Card>(FetchUrl, parameters);

            Assert.AreEqual(CacheCardName, result.Data.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(FetchUrl, It.IsAny<QueryParameters>()), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == FetchUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task WhenGetAsyncIsCalled_AndValueExistsInCache_ThenWebIsNotAccessed()
        {
            var parameters = new QueryParameters();
            var result = await TcgApiService.GetAsync<Card>(GetUrl, parameters);

            Assert.IsTrue(result.Data.Length == 1);
            Assert.AreEqual(HttpClientCardName, result.Data[0].Name);
            CacheMock.Verify(m => m.GetAsync<ApiListResponseDataContainer<Card>>(GetUrl, It.IsAny<QueryParameters>()), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Never(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == GetUrl),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task WhenFetchAsyncIsCalled_ValueIsMissingFromCache_ThenWebIsAccessed()
        {
            var parameters = new QueryParameters();
            var url = FetchNoCacheUrl;
            var result = await TcgApiService.FetchAsync<Card>(url, parameters);

            Assert.AreEqual(HttpClientCardName, result.Data.Name);
            CacheMock.Verify(m => m.GetAsync<ApiResponseDataContainer<Card>>(url, It.IsAny<QueryParameters>()), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task WhenGetAsyncIsCalled_ValueIsMissingFromCache_ThenWebIsAccessed()
        {
            var parameters = new QueryParameters();
            var url = GetNoCacheUrl;
            var result = await TcgApiService.GetAsync<Card>(url, parameters);

            Assert.IsTrue(result.Data.Length == 1);
            Assert.AreEqual(HttpClientCardName, result.Data[0].Name);
            CacheMock.Verify(m => m.GetAsync<ApiListResponseDataContainer<Card>>(url, It.IsAny<QueryParameters>()), Times.Once());
            HttpMessageHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
