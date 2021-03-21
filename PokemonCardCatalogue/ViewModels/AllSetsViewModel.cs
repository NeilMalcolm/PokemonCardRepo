using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        private List<ApiSetItem> _allSets;

        private List<ApiSetItem> _sets;
        public List<ApiSetItem> Sets
        {
            get => _sets;
            set
            {
                _sets = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set 
            { 
                if (value != _searchText  && string.IsNullOrWhiteSpace(value))
                {
                    SetDisplayList();
                }
                _searchText = value;
            }
        }

        public ICommand GoToSetCommand { get; set; }
        public ICommand AddSetToCollectionCommand { get; set; }
        public ICommand GoToSetInCollectionCommand { get; set; }
        public ICommand SearchSetsCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }


        public AllSetsViewModel(IAllSetsLogic allSetsLogic,
            ICollectionLogic collectionLogic,
            INavigationService navigationService)
            : base(navigationService)
        {
            _allSetsLogic = allSetsLogic;
            _collectionLogic = collectionLogic;
        }

        protected override async Task OnLoadAsync()
        {
            _allSets = await _allSetsLogic.GetSetsOrderedByMostRecentAsync();
            SetDisplayList();
            _canGoToSet = true;
            _canAddSetToCollection = true;
        }

        protected override void SetUpCommands()
        {
            base.SetUpCommands();

            GoToSetCommand = new Command<ApiSetItem>(async (apiSetItem) => await GoToSet(apiSetItem), (apiSetItem) => _canGoToSet);

            AddSetToCollectionCommand = new Command<ApiSetItem>
            (
                async (apiSetItem) => await AddSetToCollection(apiSetItem), 
                (apiSetItem) => !apiSetItem?.IsDownloading ?? false
            );

            GoToSetInCollectionCommand = new Command<ApiSetItem>
            (
                async (apiSetItem) => await GoToSetInCollection(apiSetItem), 
                (apiSetItem) => _canGoToSet
            );

            SearchSetsCommand = new Command<string>((searchText) => SearchSets(searchText));
            ClearSearchCommand = new Command<string>((searchText) => ClearSearch(searchText));
        }

        /// <summary>
        /// Control has no event for Cancel, so when user searches for empty text
        /// just revert list to full data set.
        /// </summary>
        /// <param name="searchText"></param>
        private void ClearSearch(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                return;
            }

            SetDisplayList();
        }

        private void SearchSets(string searchText)
        {
            SetDisplayList(searchText);
        }

        private void SetDisplayList(string searchText = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                Sets = _allSets;
                return;
            }

            Sets = _allSets
                .Where(x => x.Set.Name.Contains(searchText))
                .ToList();
        }

        private async Task GoToSet(ApiSetItem selectedSetItem)
        {
            _canGoToSet = false;
            System.Diagnostics.Debug.WriteLine($"Navigating to {selectedSetItem.Set.Name}.");

            await NavigationService.GoToAsync<SetListPage>(selectedSetItem.Set.Id);

            _canGoToSet = true;
        }

        private async Task AddSetToCollection(ApiSetItem selectedSetItem)
        {
            if (selectedSetItem.IsInCollection || selectedSetItem.IsDownloading)
            {
                return;
            }

            selectedSetItem.IsDownloading = true;
            System.Diagnostics.Debug.WriteLine($"Adding {selectedSetItem.Set.Name} to collection.");
            await _collectionLogic.AddSetAndCardsToCollection(selectedSetItem.Set);
            selectedSetItem.IsInCollection = true;
            selectedSetItem.IsDownloading = false;
        }

        private async Task GoToSetInCollection(ApiSetItem selectedSetItem)
        {
            _canGoToSet = false;
            await NavigationService.SwitchTab("collection");
            await NavigationService.GoToAsync<CollectionCardListPage>(selectedSetItem.Set.Id);
            _canGoToSet = true;
        }
    }
}
