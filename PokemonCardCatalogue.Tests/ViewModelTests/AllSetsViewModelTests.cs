using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ViewModelTests
{
    [TestClass]
    public class AllSetsViewModelTests : BaseTestClass
    {
        protected Mock<IAllSetsLogic> AllSetsLogicMock;
        protected Mock<ICollectionLogic> CollectionLogicMock;
        protected Mock<INavigationService> NavigationServiceMock;
        protected AllSetsViewModel ViewModel;

        private List<ApiSetItem> _apiSetItems = new List<ApiSetItem>
        {
            new ApiSetItem
            {
                Set = new Set
                {
                    Name = "Rising Rivals"
                }
            },
            new ApiSetItem
            {
                Set = new Set
                {
                    Name = "Guardians Rising"
                },
            },
            new ApiSetItem
            {
                Set = new Set
                {
                    Name = "Ultra Prism"
                },
            },
            new ApiSetItem
            {
                Set = new Set
                {
                    Name = "Battle Styles"
                },
            }
        };

        [TestInitialize]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        [TestCleanup]
        public override void AfterEachTest()
        {
        }

        protected override void CreateMocks()
        {
            AllSetsLogicMock = new Mock<IAllSetsLogic>();
            CollectionLogicMock = new Mock<ICollectionLogic>();
            NavigationServiceMock = new Mock<INavigationService>();
        }

        protected override void SetupMocks()
        {
            AllSetsLogicMock.Setup(m => m.GetSetsOrderedByMostRecentAsync())
                .Returns(() => Task.FromResult(_apiSetItems));
        }
        protected override void SetupData()
        {
            base.SetupData();
            ViewModel = new AllSetsViewModel
            (
                AllSetsLogicMock.Object,
                CollectionLogicMock.Object,
                NavigationServiceMock.Object
            );
        }


        [TestMethod]
        public async Task WhenLoadDataAsyncIsCalled_ThenSetsIsSetToAllSetsOrderedByDate()
        {
            await ViewModel.LoadAsync();

            Assert.AreEqual(ViewModel.Sets.Count, _apiSetItems.Count);
        }

        [TestMethod,
            DataRow(""),
            DataRow(null)
        ]
        public async Task WhenSearchTextIsSetToEmptyStringOrNull_ThenSetsCountMatchesDbResult(string value)
        {
            await ViewModel.LoadAsync();
            ViewModel.SearchText = value;

            Assert.AreEqual(ViewModel.Sets.Count, 4);
        }
        
        [TestMethod,
            DataRow("Example one"),
            DataRow("Example two")]
        public async Task WhenSearchTextIsSetToNonNullOrEmptyValue_ThenDisplaySetListIsNotChanged(string value)
        {
            await ViewModel.LoadAsync();
            var count = ViewModel.Sets.Count;
            ViewModel.SearchText = value;

            Assert.AreEqual(ViewModel.Sets.Count, count);
        }

        [TestMethod]
        public async Task WhenSearchSetsCommandIsExecuted_ThenSetsOnlyContainsResultsWithMatchingSetName()
        {
            await ViewModel.LoadAsync();
            ViewModel.SearchSetsCommand?.Execute("Ris");

            Assert.AreEqual(ViewModel.Sets.Count, 3);
        }

        [TestMethod]
        public async Task WhenClearSearchCommandIsExecuted_ThenAllSetsIsEqualToBackingStore()
        {
            await ViewModel.LoadAsync();
            ViewModel.ClearSearchCommand.Execute(null);

            Assert.AreEqual(ViewModel.Sets.Count, 4);
        }

        [TestMethod]
        public async Task WhenAddSetToCollectionCommandIsExecuted_ThenCollectionLogicAddSetAndCardsIsCalledForSet()
        {
            await ViewModel.LoadAsync();
            var selectedItem = _apiSetItems.First();
            ViewModel.AddSetToCollectionCommand.Execute(selectedItem);

            CollectionLogicMock.Verify(m => m.AddSetAndCardsToCollection(selectedItem.Set), Times.Once);
        }

        [TestMethod]
        public async Task WhenAddSetToCollectionCommandIsExecutedSeveralTimes_ThenCollectionLogicAddSetAndCardsIsCalledForSet_ExactlyOnce()
        {
            await ViewModel.LoadAsync();
            var selectedItem = _apiSetItems.First();
            ViewModel.AddSetToCollectionCommand.Execute(selectedItem);
            ViewModel.AddSetToCollectionCommand.Execute(selectedItem);
            ViewModel.AddSetToCollectionCommand.Execute(selectedItem);
            ViewModel.AddSetToCollectionCommand.Execute(selectedItem);

            CollectionLogicMock.Verify(m => m.AddSetAndCardsToCollection(selectedItem.Set), Times.AtMostOnce);
        }

        [TestMethod]
        public async Task WhenGoToSetInCollectionCommandIsExecuted_ThenTabIsSwitchedAndNavigatesToCollectionCardListPage()
        {
            await ViewModel.LoadAsync();
            var selectedItem = _apiSetItems.First();
            ViewModel.GoToSetInCollectionCommand.Execute(selectedItem);

            NavigationServiceMock.Verify(m => m.SwitchTab("collection"), Times.Once);
            NavigationServiceMock.Verify(m => m.GoToAsync<CollectionCardListPage>(selectedItem.Set), Times.Once);
        }

        [TestMethod]
        public async Task WhenGoToSetCommandIsExecuted_ThenGoToSetIsCalled()
        {
            await ViewModel.LoadAsync();
            var selectedItem = _apiSetItems.First();
            ViewModel.GoToSetCommand.Execute(selectedItem);

            NavigationServiceMock.Verify(m => m.GoToAsync<SetListPage>(selectedItem.Set), Times.Once);
        }
    }
}
