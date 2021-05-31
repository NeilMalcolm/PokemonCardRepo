using Moq;
using Moq.Protected;
using PokemonCardCatalogue.Common;
using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ServiceTests
{
    public abstract class BasePokemonTcgApiServiceTests : BaseTestFixture
    {
        protected const string FetchUrl = "http://127.0.0.1/Fetch";
        protected const string GetUrl = "http://127.0.0.1/Get";
        protected const string FetchNoCacheUrl = "http://127.0.0.1/FetchNoCache";
        protected const string GetNoCacheUrl = "http://127.0.0.1/GetNoCache";

        protected const string CacheCardName = "This is from the cache.";
        protected const string HttpClientCardName = "This is from the web.";

        protected Mock<ICache> CacheMock { get; set; }
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock { get; set; }
        protected PokemonTcgApiService TcgApiService { get; set; }

        protected override void CreateMocks()
        {
            CacheMock = new Mock<ICache>();
            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        protected override void SetupMocks()
        {
            CacheMock.Setup(m => m.GetAsync<ApiResponseDataContainer<Card>>(It.IsIn(FetchUrl), It.IsAny<QueryParameters>()))
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
            
            CacheMock.Setup(m => m.GetAsync<ApiListResponseDataContainer<Card>>(It.IsIn(GetUrl), It.IsAny<QueryParameters>()))
                .ReturnsAsync
                (
                    new ApiListResponseDataContainer<Card>
                    {
                        Data = new Card[]
                        {
                            new Card
                            {
                                Name = HttpClientCardName
                            }
                        }
                    }
                );

            HttpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>
                (
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == FetchNoCacheUrl),
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

            HttpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>
                (
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString() == GetNoCacheUrl),
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
                                new ApiListResponseDataContainer<Card>
                                {
                                    Data = new Card[]
                                    {
                                        new Card
                                        {
                                            Name = HttpClientCardName
                                        }
                                    }
                                }
                            )
                        )
                    }
                );

        }

        protected override void SetupData()
        {
            TcgApiService = new PokemonTcgApiService
            (
                CacheMock.Object,
                new HttpClient(HttpMessageHandlerMock.Object)
            );
        }
    }
}
