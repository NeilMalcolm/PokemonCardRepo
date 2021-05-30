using NUnit.Framework;
using Moq;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.ViewModelTests
{
    [TestFixture]
    public class CollectionSetsViewModelTests: BaseTestFixture
    {
        protected Mock<INavigationService> NavigationServiceMock;
        protected Mock<ICollectionLogic> CollectionLogicMock;
        protected Mock<IAlertService> AlertServiceMock;
        protected Mock<ISetListLogic> SetListLogicMock;

        private bool AlertResponse { get; set; }

        protected CollectionSetsViewModel ViewModel { get; set; }

        public List<SetItem> AllSets = new List<SetItem>
        {
            new SetItem
            {
                Set = new Set
                {
                    Id = "1",
                    Name = "Example 1"
                }
            },
            new SetItem
            {
                Set = new Set
                {
                    Id = "2",
                    Name = "Example 2"
                }
            }
        };

        private readonly SetItem SetToAdd = new SetItem
        {
            Set = new Set
            {
                Name = "Example 3"
            }
        };

        protected override void CreateMocks()
        {
            NavigationServiceMock = new Mock<INavigationService>();
            CollectionLogicMock = new Mock<ICollectionLogic>();
            AlertServiceMock = new Mock<IAlertService>();
            SetListLogicMock = new Mock<ISetListLogic>();
        }

        protected override void SetupMocks()
        {
            CollectionLogicMock.Setup(m => m.GetAllSets(true))
                .Returns
                (() =>
                   Task.FromResult(AllSets)
                );

            AlertServiceMock.Setup(m =>
                m.ShowAlertAsync
                (
                    "Confirm Delete Set",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            ).Returns(() =>
            {
                return Task.FromResult(AlertResponse);
            });


            SetListLogicMock.Setup
            (
                m => m.DoSetsAndSetFromDbHaveDifferentOwnedCounts(It.IsAny<List<SetItem>>(), AllSets[1])
            ).Returns(() =>
            {
                return true;
            });
        }

        protected override void SetupData()
        {
            ViewModel = new CollectionSetsViewModel
            (
                NavigationServiceMock.Object,
                CollectionLogicMock.Object,
                AlertServiceMock.Object,
                SetListLogicMock.Object
            );
        }

        [Test]
        public async Task WhenLoadDataIsCalled_ThenSetItemsIsSet()
        {
            await ViewModel.LoadAsync();
            Assert.IsTrue(ViewModel.SetItems.Count > 0);
        }

        [Test]
        public async Task WhenLoadDataIsCalledAnd_ThenSetItemsIsSet()
        {
            await ViewModel.LoadAsync();

        }

        [Test]
        public async Task WhenGoToSetCommandIsCalled_ThenNavigationGoToAsyncIsCalled()
        {
            await ViewModel.LoadAsync();
            ViewModel.GoToSetCommand.Execute(AllSets[0]);

            NavigationServiceMock.Verify(m => m.GoToAsync<CollectionCardListPage>(AllSets[0].Set));
        }
        
        [Test]
        public async Task WhenSetIsAdded_AndPageIsReloaded_ThenSetIsAddedToSetItems()
        {
            await ViewModel.LoadAsync();
            AllSets.Add(SetToAdd);
            await ViewModel.LoadAsync();

            Assert.IsTrue(ViewModel.SetItems.Contains(SetToAdd));
        }
        
        [Test]
        public async Task WhenCardIsAddedToSet_AndPageIsReloaded_ThenUpdateSetItemWithNewOwnedCount()
        {
            await ViewModel.LoadAsync();
            ViewModel.GoToSetCommand.Execute(AllSets[1]);

            var previousOwnedCount = AllSets[1].OwnedCount;
            AllSets[1].OwnedCount++;

            await ViewModel.LoadAsync();

            Assert.AreEqual(0, previousOwnedCount);
            Assert.AreEqual(1, AllSets[1].OwnedCount);
        }

        [Test]
        public async Task WhenDeleteSetCommandIsCalled_ThenUserIsAskedForConfirmation()
        {
            await ViewModel.LoadAsync();
            AlertResponse = false;
            ViewModel.DeleteSetCommand.Execute(AllSets[0]);

            AlertServiceMock.Verify(m => 
                m.ShowAlertAsync
                (
                    "Confirm Delete Set",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            );
        }

        [Test]
        public async Task WhenDeleteSetCommandIsCalled_AndUserDeclines_ThenSetIsNotDeleted()
        {
            await ViewModel.LoadAsync();
            AlertResponse = false;
            int setcount = ViewModel.SetItems.Count;
            ViewModel.DeleteSetCommand.Execute(AllSets[0]);

            CollectionLogicMock.Verify(m => m.DeleteSetAsync(AllSets[0].Set), Times.Never);
            Assert.AreEqual(ViewModel.SetItems.Count, setcount);
        }

        [Test]
        public async Task WhenDeleteSetCommandIsCalled_AndUserConfirmsDelete_ThenSetIsDeleted()
        {
            await ViewModel.LoadAsync();
            AlertResponse = true;
            int setcount = ViewModel.SetItems.Count;
            ViewModel.DeleteSetCommand.Execute(AllSets[0]);

            CollectionLogicMock.Verify(m => m.DeleteSetAsync(AllSets[0].Set), Times.Once);
            Assert.AreNotEqual(ViewModel.SetItems.Count, setcount);
            Assert.IsTrue(ViewModel.SetItems.Count == setcount - 1);
        }
    }
}
