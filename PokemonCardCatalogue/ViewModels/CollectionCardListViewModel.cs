using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Enums;
using PokemonCardCatalogue.Helpers;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CollectionCardListViewModel : BaseViewModel
    {
        private readonly ICollectionLogic _collectionLogic;
        private readonly IVibrationService _vibrationService;

        private DateTime? _navigatedToCardPageUtc = null;

        private int _offset = 0;
        private readonly int _limit = 39;

        private bool _canNavigateToCard = true;
        private Set _set;
        private List<CardItem> _allCardItems;

        public List<CardItem> AllCardItems 
        {
            get => _allCardItems;  
        }

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public string SetImageUrl { get; set; }

        private ObservableList<CardItem> _cardItemList;
        public ObservableList<CardItem> CardItemList
        {
            get => _cardItemList;
            set
            {
                _cardItemList = value;
                OnPropertyChanged();
            }
        }

        private KeyValuePair<SortOrder, string> _currentSortOrder = Sorting.SortModes.First();
        public KeyValuePair<SortOrder, string> CurrentSortOrder
        {
            get => _currentSortOrder;
            set
            {
                if (_currentSortOrder.Key != value.Key)
                {
                    SetDisplayList(value, _searchText);
                    _currentSortOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText && string.IsNullOrWhiteSpace(value))
                {
                    SetDisplayList(CurrentSortOrder, value);
                }

                _searchText = value;
            }
        }

        public ICommand GoToCardCommand { get; set; }
        public ICommand AddCardToCollectionCommand { get; set; }
        public ICommand LoadMoreCardItemsCommand { get; set; }
        public ICommand SearchCardsCommand { get; set; }

        public ICommand SetDisplayListCommand { get; set; }

        public CollectionCardListViewModel
            (
                INavigationService navigationService,
                ICollectionLogic collectionLogic,
                IVibrationService vibrationService
            ) 
            : base(navigationService)
        {
            _collectionLogic = collectionLogic;
            _vibrationService = vibrationService;
        }

        public override void Init(object parameter)
        {
            if (parameter is Set set)
            {
                _set = set;
                SetImageUrl = set.Images.Logo;
                Title = set.Name;
            }
        }

        protected override void SetUpCommands()
        {
            GoToCardCommand = new Command<CardItem>
            (
                async (cardItem) => await GoToCardAsync(cardItem),
                (cardItem) => _canNavigateToCard
            );
            AddCardToCollectionCommand = new Command<CardItem>
            (
                async (cardItem) => await AddCardToCollectionAsync(cardItem)
            );
            LoadMoreCardItemsCommand = new Command(LoadMore);
            SearchCardsCommand = new Command<string>(async (searchText) => await SetDisplayList(CurrentSortOrder, searchText));
        }

        protected override async Task OnLoadAsync()
        {
            _allCardItems = await _collectionLogic.GetCardsForSetAsync(_set.Id);
            await SetDisplayList(_currentSortOrder);
        }

        public async Task SetDisplayList(KeyValuePair<SortOrder, string> mode, string searchText = null)
        {
            CardItemList?.Clear();
            IsLoading = true; 

            var cards = GetCardItemsForOrderAndSearchText(mode, searchText);
            await Task.Delay(500);
            await SetCardItemList(cards);
            IsLoading = false;
        }

        private Task SetCardItemList(IEnumerable<CardItem> cardItems)
        {
            ResetOffset();
            CardItemList = new ObservableList<CardItem>(cardItems.Take(_limit));

            _offset += _limit;

            return Task.CompletedTask;
        }

        private List<CardItem> GetCardItemsForOrderAndSearchText(KeyValuePair<SortOrder, string> mode, string searchText)
        {
            if (_allCardItems is null)
            {
                return _allCardItems;
            }

            var newSortOrder = mode.Key;
            IEnumerable<CardItem> newCards = _allCardItems;

            if (!string.IsNullOrEmpty(searchText))
            {
                newCards = newCards
                    .Where(x => x.Card.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }

            newCards = newSortOrder switch
            {
                SortOrder.Rarity => newCards.OrderBy(x => x.Card.Rarity)
                                       .ThenBy(x => x.CacheId),
                SortOrder.NumericDescending => newCards.OrderByDescending(x => x.CacheId),
                _ => newCards.OrderBy(x => x.CacheId),
            };
            
            return newCards.ToList();
        }

        private void LoadMore()
        {
            if (IsLoading)
            {
                return;
            }

            var getAllCardsForOrderAndSearchText = GetCardItemsForOrderAndSearchText(CurrentSortOrder, SearchText);
            var currentCount = CardItemList.Count;
            var maxCount = getAllCardsForOrderAndSearchText.Count();

            if (currentCount == maxCount)
            {
                return;
            }

            var limit = _limit;
            if (currentCount + limit > maxCount)
            {
                limit = maxCount - currentCount;
            }

            CardItemList.AddRange
            (
                getAllCardsForOrderAndSearchText.Skip(_offset)
                    .Take(limit)
            );

            _offset += limit;
            OnPropertyChanged(nameof(CardItemList));
        }

        public override Task OnPageAppearing()
        {
            return CheckForCardChangeOnCardPagePop();
        }

        private async Task CheckForCardChangeOnCardPagePop()
        {
            if (!_navigatedToCardPageUtc.HasValue)
            {
                return;
            }

            var mostRecentChange = await _collectionLogic.GetMostRecentCardModifiedDateBySetId(_set.Id);

            if (mostRecentChange is null ||
                mostRecentChange < _navigatedToCardPageUtc)
            {
                _navigatedToCardPageUtc = null;
                return;
            }

            var cardsToUpdate = await _collectionLogic.GetMostRecentlyUpdatedCardsBySetId(_set.Id, _navigatedToCardPageUtc.Value);

            if (cardsToUpdate?.Count == 0)
            {
                _navigatedToCardPageUtc = null;
                return;
            }

            for (int i = 0; i < cardsToUpdate.Count; i++)
            {
                var cardToUpdate = cardsToUpdate[i];
                // update backing store
                var cardInList = _allCardItems.FirstOrDefault(x => x.Card.Id == cardToUpdate.Card.Id);

                var index = _allCardItems.IndexOf(cardInList);

                if (index > -1)
                {
                    _allCardItems[index] = cardToUpdate;
                }

                // update display list.
                cardInList = CardItemList.FirstOrDefault(x => x.Card.Id == cardToUpdate.Card.Id);

                if (cardInList != null)
                {
                    CardItemList.Replace(cardInList, cardToUpdate);
                }
            }

            _navigatedToCardPageUtc = null;
        }

        private async Task GoToCardAsync(CardItem cardItem)
        {
            if (!_canNavigateToCard)
            {
                return;
            }

            _canNavigateToCard = false;
            _navigatedToCardPageUtc = DateTime.UtcNow;
            cardItem.Card.Set = this._set;
            await NavigationService.GoToAsync<CardPage>(cardItem.Card);
            _canNavigateToCard = true;
        }

        private async Task AddCardToCollectionAsync(CardItem item)
        {
            if (!_canNavigateToCard || item.Owned)
            {
                return;
            }

            _vibrationService.PerformSelectionFeedbackVibration();
            item.IncrementOwnedCount();

            await semaphore.WaitAsync();

            try
            {
                _ = await _collectionLogic.QuickAddCardToCollection(item);
            }
            catch
            {

            }
            finally
            {
                semaphore.Release();
            }
        }

        private void ResetOffset()
        {
            _offset = 0;
        }
    }
}
