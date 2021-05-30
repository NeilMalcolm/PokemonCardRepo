using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ViewModelTests
{
    [TestClass]
    public class CardViewModelTests : BaseTestClass
    {
        protected Mock<ICardLogic> CardLogicMock;
        protected Mock<ICollectionLogic> CollectionLogicMock;
        protected Mock<IVibrationService> VibrationServiceMock;
        protected Mock<INavigationService> NavigationServiceMock;
        protected CardViewModel ViewModel;


        // A card with an owned count of 1.
        public Card CardWithOwnedCount1 = new Card
        {
            Id = "0"
        };
        
        // A card with related cards.
        public Card CardWithRelatedCards = new Card
        {
            Id = "1"
        };

        // A card with holo, reverse holo and normal prices.
        public static Card CardWithHolofoilReverseHoloAndNormalPrices = new Card
        {
            Id = "2",
            TcgPlayer = new TcgPlayer
            {
                Prices = new Price
                {
                    Holofoil = new TcgPlayerCardRarityType
                    {
                        High = 2.0f,
                        Mid = 1.5f,
                        DirectLow = 1.0f,
                        Market = null,
                        Low = 1.0f
                    },
                    ReverseHolofoil = new TcgPlayerCardRarityType
                    {
                        High = 1.0f,
                        Mid = 0.5f,
                        DirectLow = 0.3f,
                        Market = null,
                        Low = 0.25f
                    },
                    Normal = new TcgPlayerCardRarityType
                    {
                        High = 0.5f,
                        Mid = 0.2f,
                        DirectLow = 0.1f,
                        Market = null,
                        Low = 0.8f
                    }
                }
            }
        };

        // A card with only reverse holo and normal prices.
        public static Card CardWithOnlyReverseHoloAndNormalPrices = new Card
        {
            Id = "3",
            TcgPlayer = new TcgPlayer
            {
                Prices = new Price
                {
                    ReverseHolofoil = new TcgPlayerCardRarityType
                    {
                        High = 10.5f,
                        Mid = 7.8f,
                        DirectLow = 1.1f,
                        Market = null,
                        Low = 2.0f
                    },
                    Normal = new TcgPlayerCardRarityType
                    {
                        High = 5.0f,
                        Mid = 3.12f,
                        DirectLow = 2.1f,
                        Market = null,
                        Low = 2.0f
                    }
                }
            }
        };

        // card without any prices set.
        public Card CardWithoutAnyPrices = new Card
        {
            Id = "4"
        };

        // card where calling GetRelatedCardsInSetAsync throws an exception.
        public Card CardWhereRelatedCardGetThrowsException = new Card
        {
            Id = "5",
            TcgPlayer = new TcgPlayer
            {
                Prices = new Price
                {
                    Normal = new TcgPlayerCardRarityType
                    {
                        High = 5.0f,
                        Mid = 3.12f,
                        DirectLow = 2.1f,
                        Market = null,
                        Low = 2.0f
                    }
                }
            }
        };

        public static IEnumerable<object[]> PricesTestData
        {
            get
            {
                yield return new object[] { CardWithHolofoilReverseHoloAndNormalPrices, 3 };
                yield return new object[] { CardWithOnlyReverseHoloAndNormalPrices, 2 };
                yield return new object[] { new Card(), 0 };
            }
        }

        [TestInitialize]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        [TestCleanup]
        public override void AfterEachTest()
        {
            base.AfterEachTest();
        }

        protected override void CreateMocks()
        {
            CardLogicMock = new Mock<ICardLogic>();
            CollectionLogicMock = new Mock<ICollectionLogic>();
            VibrationServiceMock = new Mock<IVibrationService>();
            NavigationServiceMock = new Mock<INavigationService>();
        }

        protected override void SetupMocks()
        {
            CollectionLogicMock.Setup(m => m.GetCardNormalOwnedCount(CardWithOwnedCount1.Id))
                .Returns(Task.FromResult(1));
            
            CollectionLogicMock.Setup(m => m.GetCardNormalOwnedCount(CardWhereRelatedCardGetThrowsException.Id))
                .Returns(Task.FromResult(5));

            CardLogicMock.Setup(m => m.GetRelatedCardsInSetAsync(CardWithRelatedCards))
                .Returns(Task.FromResult(new List<Card>
                {
                    new Card(),
                    new Card(),
                }));

            CardLogicMock.Setup(m => m.GetRelatedCardsInSetAsync(CardWhereRelatedCardGetThrowsException))
                .Throws(new Exception("Expected Exception"));
        }

        protected override void SetupData()
        {
            ViewModel = new CardViewModel
            (
                NavigationServiceMock.Object,
                CardLogicMock.Object,
                CollectionLogicMock.Object,
                VibrationServiceMock.Object
            );
        }

        [TestMethod]
        public async Task WhenDecrementOwnedCountCommandIsExecuted_AndOwnedCountIsAlreadyZero_ThenCountNotDecreased()
        {
            ViewModel.Init(new Card());
            await ViewModel.LoadAsync();
            ViewModel.DecrementNormalOwnedCountCommand.Execute(null);
            Assert.AreEqual(ViewModel.NormalOwnedCount, 0);
        }

        [TestMethod]
        public async Task WhenDecrementOwnedCountCommandIsExecuted_AndOwnedCountIsGreaterThanZero_ThenCountIsDecreased()
        {
            ViewModel.Init(CardWithOwnedCount1);
            await ViewModel.LoadAsync();
            ViewModel.DecrementNormalOwnedCountCommand.Execute(null);
            Assert.AreEqual(ViewModel.NormalOwnedCount, 0);
        }
        
        [TestMethod,
            DataRow(1),
            DataRow(2),
            DataRow(5)]
        public async Task WhenDecrementOwnedCountCommandIsExecutedMultipleTimes_AndOwnedCountIsGreaterThanZero_ThenCountIsDecreased(int numberOfDecrements)
        {
            ViewModel.Init(CardWithOwnedCount1);
            await ViewModel.LoadAsync();

            for (int i = 0; i < numberOfDecrements; i++)
            {
                ViewModel.DecrementNormalOwnedCountCommand.Execute(null);
            }

            Assert.AreEqual(ViewModel.NormalOwnedCount, 0);
        }

        [TestMethod]
        public async Task WhenIncrementOwnedCountCommandIsExecuted_ThenCountIsIncremented()
        {
            ViewModel.Init(new Card());
            await ViewModel.LoadAsync();
            ViewModel.IncrementNormalOwnedCountCommand.Execute(null);
            Assert.AreEqual(ViewModel.NormalOwnedCount, 1);
        }
        
        [TestMethod,
            DataRow(1),
            DataRow(2),
            DataRow(5)]
        public async Task WhenIncrementOwnedCountCommandIsExecutedMultipleTimes_ThenCountIsIncrementedSameAmountOfTimes(int numberOfIncrements)
        {
            ViewModel.Init(new Card());
            await ViewModel.LoadAsync();

            for (int i = 0; i < numberOfIncrements; i++)
            {
                ViewModel.IncrementNormalOwnedCountCommand.Execute(null);
            }

            Assert.AreEqual(ViewModel.NormalOwnedCount, numberOfIncrements);
        }

        [TestMethod]
        public async Task WhenOnLoadAsyncIsCalled_ThenRelatedCardsAreLoaded()
        {
            ViewModel.Init(CardWithRelatedCards);
            await ViewModel.LoadAsync();

            Assert.IsTrue(ViewModel.RelatedCards.Count > 0);
        }

        [TestMethod]
        public async Task WhenGoToRelatedCardCommandIsExecuted_ThenNavigationServiceIsCalled()
        {
            ViewModel.Init(CardWithRelatedCards);
            await ViewModel.LoadAsync();

            var selectedRelatedCard = ViewModel.RelatedCards.First();
            ViewModel.GoToRelatedCardCommand.Execute(selectedRelatedCard);

            NavigationServiceMock.Verify(m => m.GoToAsync<CardPage>(selectedRelatedCard), Times.Once);
        }

        [TestMethod]
        public async Task WhenOnLoadAsyncIsCalled_AndRelatedCardLoadThrowsException_ThenLoadingStopsAndPricesAndOwnedCountStillSet()
        {
            ViewModel.Init(CardWhereRelatedCardGetThrowsException);
            await ViewModel.LoadAsync();

            CardLogicMock.Verify(m => m.GetRelatedCardsInSetAsync(CardWhereRelatedCardGetThrowsException), Times.Once);
            CollectionLogicMock.Verify(m => m.GetCardNormalOwnedCount(CardWhereRelatedCardGetThrowsException.Id), Times.Once);

            Assert.IsNotNull(ViewModel.Prices);
            Assert.AreEqual(ViewModel.Prices.Count, 1);
            Assert.AreEqual(ViewModel.NormalOwnedCount, 5);
        }

        [TestMethod]
        public async Task WhenOnLoadAsyncIsCalled_ThenOwnedCountIsLoaded()
        {
            ViewModel.Init(CardWithOwnedCount1);
            await ViewModel.LoadAsync();

            Assert.AreEqual(ViewModel.NormalOwnedCount, 1);
        }

        [DataTestMethod]
        [DynamicData(nameof(PricesTestData))]
        public async Task WhenOnLoadAsyncIsCalled_AndCardPrices_ThenCorrectNumberOfPricesAreSet(Card card, int expectedCount)
        {
            ViewModel.Init(card);
            await ViewModel.LoadAsync();

            Assert.AreEqual(ViewModel.Prices.Count, expectedCount);
        }
    }
}
