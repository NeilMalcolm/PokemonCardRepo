using Moq;
using NUnit.Framework;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ViewModelTests
{
    [TestFixture]
    public class CollectionCardListViewModelTests : BaseTestFixture
    {
        private const int _secondSetCardCount = 125;

        protected Mock<ICollectionLogic> CollectionLogicMock;
        protected Mock<IVibrationService> VibrationServiceMock;
        protected Mock<INavigationService> NavigationServiceMock;
        protected Mock<ILog> LogMock;

        public CollectionCardListViewModel ViewModel;

        public Set FirstSet = new Set
        {
            Id = "0",
            Images = new SetImages
            {

            }
        };
        
        public Set SecondSet = new Set
        {
            Id = "1",
            Images = new SetImages
            {

            }
        };

        public List<CardItem> FirstSetCards = new List<CardItem>
        {
            new CardItem
            {
                CacheId = 0,
                Card = new Card
                {
                    Id = "0",
                    Number = "1",
                    Name = "Charmander"
                }
            },
            new CardItem
            {
                CacheId = 1,
                Card = new Card
                {
                    Id = "1",
                    Number = "2",
                    Name = "Charmeleon"
                }
            },
            new CardItem
            {
                CacheId = 2,
                Card = new Card
                {
                    Id = "2",
                    Number = "3",
                    Name = "Charizard"
                }
            },
            new CardItem
            {
                CacheId = 3,
                Card = new Card
                {
                    Id = "3",
                    Number = "4",
                    Name = "Squirtle"
                }
            }
        };

        public List<CardItem> SecondSetCards;

        protected override void CreateMocks()
        {
            CollectionLogicMock = new Mock<ICollectionLogic>();
            VibrationServiceMock = new Mock<IVibrationService>();
            NavigationServiceMock = new Mock<INavigationService>();
            LogMock = new Mock<ILog>();
        }

        protected override void SetupMocks()
        {
            CollectionLogicMock.Setup(m => m.GetCardsForSetAsync(FirstSet.Id))
               .Returns(Task.FromResult(FirstSetCards));
            
            CollectionLogicMock.Setup(m => m.GetCardsForSetAsync(SecondSet.Id))
               .Returns(() =>
               {
                   BuildSecondSetCards();
                   return Task.FromResult(SecondSetCards);
               });

            CollectionLogicMock.Setup(m => m.GetMostRecentCardModifiedDateBySetId(FirstSet.Id))
                .Returns
                (
                    Task.FromResult<DateTime?>(DateTime.UtcNow.AddMinutes(1))
                );
            CollectionLogicMock.Setup(m => m.GetMostRecentlyUpdatedCardsBySetId(FirstSet.Id, It.IsAny<DateTime>()))
                .ReturnsAsync
                (() =>
                {
                    var newCard = FirstSetCards[0];
                    newCard.NormalOwnedCount += 1;
                    return new List<CardItem>
                    {
                        newCard
                    };
                });
        }

        protected override void SetupData()
        {
            ViewModel = new CollectionCardListViewModel
            (
               NavigationServiceMock.Object,
               CollectionLogicMock.Object,
               VibrationServiceMock.Object,
               LogMock.Object
            );
        }

        [Test]
        public async Task WhenOnLoadAsyncIsCalled_ThenDisplayListIsSet()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            Assert.AreEqual(ViewModel.AllCardItems.Count, FirstSetCards.Count);
        }

        [Test]
        public async Task WhenSetDisplayListIsCalled_AndModeIsSetButSearchTextIsNull_ThenResultsAreSorted()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            await ViewModel.SetDisplayList(Sorting.SortModes[1]);

            Assert.AreEqual(ViewModel.AllCardItems.Last(), ViewModel.CardItemList.First());
            Assert.AreEqual(ViewModel.AllCardItems.First(), ViewModel.CardItemList.Last());
        }

        [Test]
        public async Task WhenSetDisplayListIsCalled_AndModeIsDefaultAndSearchTextIsSet_ThenResultsAreSortedAndSearched()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            // numerical asc sort order + 'char' search text
            await ViewModel.SetDisplayList(Sorting.SortModes[0], "char"); 

            Assert.AreEqual(3, ViewModel.CardItemList.Count);
            Assert.IsTrue
            (
                ViewModel.CardItemList.All(m => m.Card.Name.Contains("char", StringComparison.OrdinalIgnoreCase))
            );
        }

        [Test]
        public async Task WhenSetDisplayListIsCalled_AndModeIsSetAndSearchTextIsSet_ThenResultsAreSortedAndSearched()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            // numerical desc sort order + 'char' search text
            await ViewModel.SetDisplayList(Sorting.SortModes[1], "char"); 

            Assert.AreEqual(3, ViewModel.CardItemList.Count);
            Assert.IsTrue
            (
                ViewModel.CardItemList.All(m => m.Card.Name.Contains("char", StringComparison.OrdinalIgnoreCase))
            );
            Assert.IsTrue(ViewModel.CardItemList[0].Card.Name == "Charizard");
            Assert.IsTrue(ViewModel.CardItemList[1].Card.Name == "Charmeleon");
            Assert.IsTrue(ViewModel.CardItemList[2].Card.Name == "Charmander");
        }

        [Test]
        public async Task WhenGoToCardAsyncIsCalled_ThenNavigationToCardPageIsDone()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            ViewModel.GoToCardCommand.Execute(FirstSetCards[0]);

            NavigationServiceMock.Verify(m => m.GoToAsync<CardPage>(FirstSetCards[0].Card));
        }

        [Test]
        public async Task WhenOnPageAppearingIsCalled_AndCardPageWasNavigatedTo_ThenReloadUpdatedCard()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            ViewModel.GoToCardCommand.Execute(FirstSetCards[0]);
            var ownedCount = ViewModel.CardItemList[0].NormalOwnedCount;
            await ViewModel.OnPageAppearing();

            Assert.AreNotEqual(ownedCount, ViewModel.CardItemList[0].NormalOwnedCount);
        }

        [Test]
        public async Task WhenLoadMoreCardItemsCommandIsCalled_AndMoreCanBeLoaded_ThenCardItemListCoundIncreases()
        {
            ViewModel.Init(SecondSet);
            await ViewModel.LoadAsync();

            var initialCount = ViewModel.CardItemList.Count;
            ViewModel.LoadMoreCardItemsCommand.Execute(null);
            var newCount = ViewModel.CardItemList.Count;

            Assert.IsTrue(newCount > initialCount);
        }
        
        [Test]
        public async Task WhenLoadMoreCardItemsCommandIsCalled_AndRemainingCardsIsLessThanLimit_AllCardsAreLoadedAndNoExceptions()
        {
            ViewModel.Init(SecondSet);
            await ViewModel.LoadAsync();

            var initialCount = ViewModel.CardItemList.Count;
            ViewModel.LoadMoreCardItemsCommand.Execute(null);
            ViewModel.LoadMoreCardItemsCommand.Execute(null);
            var newCount = ViewModel.CardItemList.Count;
            ViewModel.LoadMoreCardItemsCommand.Execute(null);
            var finalCount = ViewModel.CardItemList.Count;

            Assert.IsTrue(initialCount < finalCount);
            Assert.IsTrue(initialCount < newCount);
            Assert.IsTrue(newCount < finalCount);

            Assert.AreEqual(ViewModel.CardItemList.Count, _secondSetCardCount);
        }

        private void BuildSecondSetCards()
        {
            SecondSetCards = new List<CardItem>();

            for (int i = 0; i < _secondSetCardCount; i++)
            {
                SecondSetCards.Add(new CardItem
                {
                    CacheId = i,
                    Card = new Card
                    {
                        Id = $"{i}"
                    }
                });
            }
        }
    }
}
