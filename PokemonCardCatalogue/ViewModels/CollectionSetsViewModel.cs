using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CollectionSetsViewModel : BaseViewModel
    {
        private readonly ICollectionLogic _collectionLogic;
        private readonly ISetListLogic _setListLogic;
        private readonly IAlertService _alertService;

        private readonly List<SetItem> _deletingSetItems = new List<SetItem>();

        private SetItem _previouslyNavigatedToSetItem;
        private bool _canGoToSet = true;

        private ObservableCollection<SetItem> _setItems;
        public ObservableCollection<SetItem> SetItems
        {
            get => _setItems;
            set
            {
                _setItems = value;
                OnPropertyChanged();
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

        private float _collectionCardsMaxMarketTotal;
        public float CollectionCardsMaxMarketTotal
        {
            get => _collectionCardsMaxMarketTotal;
            set
            {
                _collectionCardsMaxMarketTotal = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteSetCommand { get; set; }
        public ICommand GoToSetCommand { get; set; } 

        public CollectionSetsViewModel(INavigationService navigationService,
            ICollectionLogic collectionLogic,
            IAlertService alertService,
            ISetListLogic setListLogic) 
            : base(navigationService)
        {
            _collectionLogic = collectionLogic;
            _alertService = alertService;
            _setListLogic = setListLogic;
            ReloadDataOnAppearing = true;
        }

        protected override void SetUpCommands()
        {
            RefreshCommand = new Command(async () => await RefreshAsync(), () => !IsRefreshing);
            DeleteSetCommand = new Command<SetItem>
            (
                async (setItem) => await ConfirmDeleteSet(setItem), 
                (setItem) => CanDeleteSet(setItem)
            );
            GoToSetCommand = new Command<SetItem>
            (
                async (setItem) => await GoToSetAsync(setItem),
                (setItem) => _canGoToSet
            );
        }

        protected override Task OnLoadAsync()
        {
            return LoadAllData();
        }
        
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            try
            {
                await LoadAllData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private Task LoadAllData()
        {
            var allSetsTask = LoadSetsAsync();
            var totalPriceTask = LoadCollectionCardsMaxMarketTotalAsync();

            return Task.WhenAll(allSetsTask, totalPriceTask);
        }

        private async Task LoadSetsAsync()
        {
            var allSets = await _collectionLogic.GetAllSets(withCount: true);

            if (allSets.Count != SetItems?.Count)
            {
                // if new sets have been added (or sets have been removed) update list.
                SetItems = new ObservableCollection<SetItem>(allSets);
            }
            else if (_setListLogic.DoSetsAndSetFromDbHaveDifferentOwnedCounts(allSets, _previouslyNavigatedToSetItem))
            {
                if (_previouslyNavigatedToSetItem != null)
                {
                    // if we previously navigated to a set, replace the set to capture any changes.
                    var item = allSets?.FirstOrDefault(x => x.Set.Id == _previouslyNavigatedToSetItem.Set.Id);
                    var index = SetItems.IndexOf(_previouslyNavigatedToSetItem);
                    SetItems.RemoveAt(index);
                    SetItems.Insert(index, item);
                }
            }

            _previouslyNavigatedToSetItem = null;
        }

        private async Task LoadCollectionCardsMaxMarketTotalAsync()
        {
            CollectionCardsMaxMarketTotal = await _collectionLogic.GetMaxMarketValueForCollection();
        }

        private async Task ConfirmDeleteSet(SetItem setItem)
        {
            var shouldDelete = await _alertService.ShowAlertAsync
            (
                "Confirm Delete Set", 
                $"You are about to delete {setItem.Set.Name} from your collection. This will remove all cards too. Do you want to delete?",
                "Delete", 
                "Do not delete"
            );

            if (!shouldDelete)
            {
                return;
            }

            await DeleteSet(setItem);
        }

        private async Task GoToSetAsync(SetItem item)
        {
            _canGoToSet = false;
            _previouslyNavigatedToSetItem = item;
            await NavigationService.GoToAsync<CollectionCardListPage>(item.Set);
            _canGoToSet = true;
        }

        private async Task DeleteSet(SetItem setItem)
        {
            _deletingSetItems.Add(setItem);
            await _collectionLogic.DeleteSetAsync(setItem.Set);
            _deletingSetItems.Remove(setItem);
            SetItems.Remove(setItem);
        }

        private bool CanDeleteSet(SetItem setItem)
        {
            return !_deletingSetItems.Any(s => s.Set.Id == setItem.Set.Id);
        }
    }
}
