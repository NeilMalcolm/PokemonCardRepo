using Moq;
using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using System.Net.Http;

namespace PokemonCardCatalogue.Tests.ServiceTests
{
    public abstract class BasePokemonTcgApiServiceTests : BaseTestFixture
    {
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
