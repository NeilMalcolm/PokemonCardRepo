using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class AllSetsViewModel : BaseViewModel
    {
        private readonly IAllSetsLogic _allSetsLogic;
        private readonly ICollectionLogic _collectionLogic;

        private bool _canGoToSet;
        private bool _canAddSetToCollection;

        private List<Set> _sets;
        public List<Set> Sets
        {
            get => _sets;
            set
            {
                _sets = value;
                OnPropertyChanged();
            }
        }

        private ICommand _goToSetCommand;

        public ICommand GoToSetCommand
        {
            get { return _goToSetCommand; }
            set { _goToSetCommand = value; }
        }

        private ICommand _addSetToCollectionCommand;

        public ICommand AddSetToCollectionCommand
        {
            get { return _addSetToCollectionCommand; }
            set { _addSetToCollectionCommand = value; }
        }

        public AllSetsViewModel(IAllSetsLogic allSetsLogic,
            INavigationService navigationService,
            ICollectionLogic collectionLogic)
            : base(navigationService)
        {
            _allSetsLogic = allSetsLogic;
            _collectionLogic = collectionLogic;
        }

        protected override async Task OnLoadAsync()
        {
            Sets = await _allSetsLogic.GetSetsOrderedByMostRecentAsync();
            _canGoToSet = true;
            _canAddSetToCollection = true;
        }

        protected override void SetUpCommands()
        {
            base.SetUpCommands();

            GoToSetCommand = new Command<Set>(async (set) => await GoToSet(set), (set) => _canGoToSet);
            AddSetToCollectionCommand = new Command<Set>(async (set) => await AddSetToCollection(set), (set) => _canAddSetToCollection);
        }

        private async Task GoToSet(Set selectedSet)
        {
            _canGoToSet = false;
            System.Diagnostics.Debug.WriteLine($"Navigating to {selectedSet.Name}.");

            await NavigationService.GoToAsync<SetListPage>(selectedSet.Id);

            _canGoToSet = true;
        }

        private async Task AddSetToCollection(Set selectedSet)
        {
            _canAddSetToCollection = false;

            System.Diagnostics.Debug.WriteLine($"Adding {selectedSet.Name} to collection.");
            await _collectionLogic.AddSetAndCardsToCollection(selectedSet);

            _canAddSetToCollection = true;
        }
    }
}
