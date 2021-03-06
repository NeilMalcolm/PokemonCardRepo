using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
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
                // Since SearchTextComamnd searches for us,
                // this is only required to display all results 
                // text when empty.
                if (value != _searchText  && string.IsNullOrWhiteSpace(value))
                {
                    SetDisplayList();
                }
                _searchText = value;
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing; 
            set 
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private string _emptyListMessage;
        public string EmptyListMessage
        {
            get => _emptyListMessage;
            set
            {
                _emptyListMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToSetCommand { get; set; }
        public ICommand AddSetToCollectionCommand { get; set; }
        public ICommand GoToSetInCollectionCommand { get; set; }
        public ICommand SearchSetsCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }
        public ICommand ForceGetLatestCardSetsDataCommand { get; set; }

        public AllSetsViewModel
        (
            IAllSetsLogic allSetsLogic,
            ICollectionLogic collectionLogic,
            INavigationService navigationService,
            ILog log
        ) : base(navigationService, log)
        {
            _allSetsLogic = allSetsLogic;
            _collectionLogic = collectionLogic;
        }

        protected override async Task OnLoadAsync()
        {
            try
            {
                _allSets = await _allSetsLogic.GetSetsOrderedByMostRecentAsync();
                SetDisplayList();
                _canGoToSet = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                
                EmptyListMessage = ErrorMessages.SetList.CollectionViewWebRequestTimeoutMessage;
            }
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
            ForceGetLatestCardSetsDataCommand = new Command(async () => await ForceGetLatestCardSetsData(), () => !IsRefreshing);
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
                .Where(x => x.Set.Name.Contains(searchText, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private async Task GoToSet(ApiSetItem selectedSetItem)
        {
            _canGoToSet = false;
            System.Diagnostics.Debug.WriteLine($"Navigating to {selectedSetItem.Set.Name}.");

            await NavigationService.GoToAsync<SetListPage>(selectedSetItem.Set);

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

            try
            {
                await _collectionLogic.AddSetAndCardsToCollection(selectedSetItem.Set);
                selectedSetItem.IsInCollection = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                selectedSetItem.IsDownloading = false;
            }
        }

        private async Task GoToSetInCollection(ApiSetItem selectedSetItem)
        {
            _canGoToSet = false;
            await NavigationService.SwitchTab("collection");
            await NavigationService.GoToAsync<CollectionCardListPage>(selectedSetItem.Set);
            _canGoToSet = true;
        }

        private async Task ForceGetLatestCardSetsData()
        {
            IsRefreshing = true;

            try
            {
                _allSets = await _allSetsLogic.GetSetsOrderedByMostRecentAsync(true);
                await Task.Delay(500);
                SearchText = string.Empty;
                Sets = _allSets;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}