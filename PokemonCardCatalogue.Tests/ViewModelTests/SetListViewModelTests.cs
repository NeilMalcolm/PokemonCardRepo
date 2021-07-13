using Moq;
using NUnit.Framework;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ViewModelTests
{
    [TestFixture]
    public class SetListViewModelTests : BaseTestFixture
    {
        protected Mock<INavigationService> NavigationServiceMock;
        protected Mock<ISetListLogic> SetListLogicMock;
        protected Mock<ILog> LogMock;

        public Set FirstSet = new Set
        {
            Id = "1",
            Images =  new SetImages
            {

            }
        };

        private readonly List<Card> FirstSetCards = new List<Card>
        {
            new Card
            {
                Id = "1"
            },
            new Card
            {
                Id = "2"
            }
        };

        public SetListViewModel ViewModel { get; set; }

        [SetUp]
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
            NavigationServiceMock = new Mock<INavigationService>();
            SetListLogicMock = new Mock<ISetListLogic>();
            LogMock = new Mock<ILog>();
        }

        protected override void SetupMocks()
        {
            SetListLogicMock.Setup(m => m.GetAllCardsForSetAsync(FirstSet.Id))
                .Returns(() =>
                {
                    return Task.FromResult(FirstSetCards);
                });
        }

        protected override void SetupData()
        {
            ViewModel = new SetListViewModel
            (
                SetListLogicMock.Object,
                NavigationServiceMock.Object,
                LogMock.Object
            );
        }

        [Test]
        public async Task WhenOnLoadAsyncIsCalled_ThenCardListIsSet()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();

            Assert.AreEqual(2, ViewModel.CardList.Count);
        }


        [Test]
        public async Task WhenGoToCardAsyncIsCalled_ThenNavigationServiceIsCalled()
        {
            ViewModel.Init(FirstSet);
            await ViewModel.LoadAsync();
            ViewModel.GoToCardCommand.Execute(FirstSetCards[0]);

            NavigationServiceMock.Verify(m => m.GoToAsync<CardPage>(FirstSetCards[0]), Times.Once);
        }
    }
}
