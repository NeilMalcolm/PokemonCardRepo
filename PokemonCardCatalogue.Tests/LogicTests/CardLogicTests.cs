using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models.Types;
using PokemonCardCatalogue.Logic;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq.Protected;
using System;
using System.Net;
using System.Threading;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common;

namespace PokemonCardCatalogue.Tests.LogicTests
{
    [TestClass]
    public class CardLogicTests : BaseTestClass
    {
        private const string HttpClientBaseAddress = "http://127.0.0.1";

        protected Mock<HttpClientHandler> HttpClientHandlerMock;
        protected HttpClient HttpClient;
        protected Mock<ICache> CacheMock;

        protected IApi Api;
        protected IApiService ApiService;

        protected ICardLogic Logic { get; set; }

        public static IEnumerable<object[]> GetPokemonRelatedCardRequestCardsAndExpectedUrls
        {
            get
            {
                yield return new object[]
                {
                    new Card
                    {
                        Id = "1",
                        Name = "Venusaur V",
                        Supertype = Supertypes.Pokemon,
                        NationalPokedexNumbers = new int[] { 1 },
                        Set = new Set { Id = "1" }
                    },
                    $"{HttpClientBaseAddress}/cards?q=(nationalPokedexNumbers:[1 TO 1]) set.id:1 -id:1&orderBy=number"
                };

                yield return new object[]
                {
                    new Card
                    {
                        Id = "2",
                        Name = "Venusaur VMAX",
                        Supertype = Supertypes.Pokemon,
                        NationalPokedexNumbers = new int[] { 1 },
                        Set = new Set { Id = "1" }
                    },
                    $"{HttpClientBaseAddress}/cards?q=(nationalPokedexNumbers:[1 TO 1]) set.id:1 -id:2&orderBy=number"
                };
            }
        }
        
        public static IEnumerable<object[]> GetNonPokemonRelatedCardRequestCardsAndExpectedUrls
        {
            get
            {
                yield return new object[]
                {
                    new Card
                    {
                        Id = "1",
                        Name = "Professor's Research",
                        Set = new Set { Id = "1" }
                    },
                    $"{HttpClientBaseAddress}/cards?q=name:*Professor's* -id:1 set.id:1&orderBy=number"
                };

                yield return new object[]
                {
                    new Card
                    {
                        Id = "2",
                        Name = "Gym Trainer",
                        Set = new Set { Id = "1" }
                    },
                    $"{HttpClientBaseAddress}/cards?q=name:*Gym* -id:2 set.id:1&orderBy=number"
                };
            }
        }
        
        public static IEnumerable<object[]> PokemonInCache
        {
            get
            {
                yield return new object[]
                {
                    new Card
                    {
                        Id = "4",
                        Name = "Venusaur V",
                        Supertype = Supertypes.Pokemon,
                        NationalPokedexNumbers = new int[] { 1 },
                        Set = new Set { Id = "1" }
                    },
                };

                yield return new object[]
                {
                    new Card
                    {
                        Id = "5",
                        Name = "Venusaur VMAX",
                        Supertype = Supertypes.Pokemon,
                        NationalPokedexNumbers = new int[] { 1 },
                        Set = new Set { Id = "1" }
                    },
                };
            }
        }


        [TestInitialize]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        public override void AfterEachTest()
        {
            base.AfterEachTest();
        }

        protected override void CreateMocks()
        {
            HttpClientHandlerMock = new Mock<HttpClientHandler>();
            HttpClient = new HttpClient(HttpClientHandlerMock.Object)
            {
                BaseAddress = new Uri(HttpClientBaseAddress)
            };

            CacheMock = new Mock<ICache>();
        }

        protected override void SetupMocks()
        {
            HttpClientHandlerMock.Protected().Setup<Task<HttpResponseMessage>>
            (
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            })
            .Verifiable();

            CacheMock.Setup
            (
                m => m.GetAsync<ApiListResponseDataContainer<Card>>
                (
                    "cards",
                    It.Is<QueryParameters>(m => m.Query["-id"] == "4"
                    || m.Query["-id"] == "5")
                )
            )
            .Returns(Task.FromResult(new ApiListResponseDataContainer<Card>()));
        }

        protected override void SetupData()
        {
            ApiService = new PokemonTcgApiService
            (
                CacheMock.Object,
                HttpClient
            );

            Api = new PokemonTcgApi
            (
                CacheMock.Object,
                ApiService
            );

            Logic = new CardLogic
            (
                Api
            );
        }

        [DataTestMethod,
            DataRow(null)]
        public async Task WhenGetRelatedCardsInSetAsyncIsCalled_AndCardIsNull_ThenApiReturnsEmptyList(Card card)
        {
            var result = await Logic.GetRelatedCardsInSetAsync(card);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [DataTestMethod,
            DynamicData(nameof(GetPokemonRelatedCardRequestCardsAndExpectedUrls))]
        public Task WhenGetPokemonRelatedCardsInSetAsyncIsCalled_ThenApiCallsExpectedUrl(Card card, string expectedRequestUrl)
        {
            return GetExpectedRelatedCardResults(card, expectedRequestUrl);
        }

        [DataTestMethod,
            DynamicData(nameof(GetNonPokemonRelatedCardRequestCardsAndExpectedUrls))]
        public Task WhenGetNonPokemonRelatedCardsInSetAsyncIsCalled_ThenApiCallsExpectedUrl(Card card, string expectedRequestUrl)
        {
            return GetExpectedRelatedCardResults(card, expectedRequestUrl);
        }

        [DataTestMethod,
            DynamicData(nameof(PokemonInCache))]
        public async Task WhenGetNonPokemonRelatedCardsInSetAsyncIsCalled_AndExistsInCache_ThenApiIsNeverCalled(Card card)
        {
            var result = await Logic.GetRelatedCardsInSetAsync(card);

            CacheMock.Verify(m =>
                m.GetAsync<ApiListResponseDataContainer<Card>>
                (
                    "cards",
                    It.Is<QueryParameters>(m => m.Query["-id"] == card.Id)
                ),
                Times.Once
            );

            HttpClientHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        private async Task GetExpectedRelatedCardResults(Card card, string expectedRequestUrl)
        {
            var result = await Logic.GetRelatedCardsInSetAsync(card);

            HttpClientHandlerMock.Protected().Verify
            (
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>
                (
                    m => m.Method == HttpMethod.Get
                    && m.RequestUri.ToString() == expectedRequestUrl
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
     }
}
