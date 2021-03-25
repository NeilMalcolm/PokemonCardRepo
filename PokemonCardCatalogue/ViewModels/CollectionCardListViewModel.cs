﻿using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public enum SortOrder
    {
        Unspecified,
        NumericAscending,
        NumericDescending,
        Rarity
    }

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

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private List<KeyValuePair<SortOrder, string>> _sortModes;
        public List<KeyValuePair<SortOrder, string>> SortModes
        {
            get
            {
                return _sortModes ??= new List<KeyValuePair<SortOrder, string>>
                {
                    new KeyValuePair<SortOrder, string>(SortOrder.NumericAscending, "Number Asc"),
                    new KeyValuePair<SortOrder, string>(SortOrder.NumericDescending, "Number Desc"),
                    new KeyValuePair<SortOrder, string>(SortOrder.Rarity, "Rarity"),
                };
            }
        }

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

        private KeyValuePair<SortOrder, string> _currentSortOrder;
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
            SearchCardsCommand = new Command<string>((searchText) => SetDisplayList(CurrentSortOrder, searchText));
        }

        protected override async Task OnLoadAsync()
        {
            _allCardItems = await _collectionLogic.GetCardsForSetAsync(_set.Id);

            SetDisplayList(SortModes.FirstOrDefault());
        }

        private void SetDisplayList(KeyValuePair<SortOrder, string> mode, string searchText = null)
        {
            Device.BeginInvokeOnMainThread(() => 
            {
                CardItemList?.Clear();
                IsLoading = true; 
            });

            var cards = GetCardItemsForOrderAndSearchText(mode, searchText);
            Task.Run(async () =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await SetCardItemList(cards);
                    IsLoading = false;
                });
            });
        }

        private Task SetCardItemList(IEnumerable<CardItem> cardItems)
        {
            ResetOffset();
            CardItemList = new ObservableList<CardItem>(cardItems
                .Take(_limit));

            _offset += _limit;

            return Task.CompletedTask;
        }

        private List<CardItem> GetCardItemsForOrderAndSearchText(KeyValuePair<SortOrder, string> mode, string searchText)
        {
            var newSortOrder = mode.Key;
            IEnumerable<CardItem> newCards = _allCardItems;

            if (!string.IsNullOrEmpty(searchText))
            {
                newCards = newCards
                    .Where(x => x.Card.Name.Contains(searchText, System.StringComparison.OrdinalIgnoreCase));
            }

            switch (newSortOrder)
            {
                case SortOrder.Rarity:
                    newCards = newCards.OrderBy(x => x.Card.Rarity)
                        .ThenBy(x => x.CacheId);
                    break;
                case SortOrder.NumericDescending:
                    newCards = newCards.OrderByDescending(x => x.CacheId);
                    break;
                default:
                case SortOrder.NumericAscending:
                    newCards = newCards.OrderBy(x => x.CacheId);
                    break;
            }

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

        public override async Task OnPageAppearing()
        {
            if (_navigatedToCardPageUtc != null)
            {
                var mostRecentChange = await _collectionLogic.GetMostRecentCardModifiedDateBySetId(_set.Id);

                if (mostRecentChange is null || 
                    mostRecentChange < _navigatedToCardPageUtc)
                {
                    _navigatedToCardPageUtc = null;
                    return;
                }

                var cardToUpdate = await _collectionLogic.GetMostRecentlyUpdatedCardBySetId(_set.Id);
                
                if (cardToUpdate is null)
                {
                    _navigatedToCardPageUtc = null;
                    return;
                }

                // update backing store
                var cardInList = _allCardItems.FirstOrDefault(x => x.Card.Id == cardToUpdate.Card.Id);
                var index = _allCardItems.IndexOf(cardInList);

                if(index > -1)
                {
                    _allCardItems[index] = cardToUpdate;
                }

                cardInList = CardItemList.FirstOrDefault(x => x.Card.Id == cardToUpdate.Card.Id);

                if (index != null)
                {
                    CardItemList.Replace(cardInList, cardToUpdate);
                }
                _navigatedToCardPageUtc = null;
            }
        }

        private async Task GoToCardAsync(CardItem cardItem)
        {
            if (!_canNavigateToCard)
            {
                return;
            }

            _canNavigateToCard = false;
            _navigatedToCardPageUtc = DateTime.UtcNow;
            await NavigationService.GoToAsync<CardPage>(cardItem.Card);
            _canNavigateToCard = true;
        }

        private async Task AddCardToCollectionAsync(CardItem item)
        {
            if (!_canNavigateToCard || item.OwnedCount > 0)
            {
                return;
            }

            _vibrationService.PerformNotificationFeedbackVibration();
            item.IncrementOwnedCount();

            await semaphore.WaitAsync();

            try
            {
                _ = await _collectionLogic.SetOwnedCountForCard(item);
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

    public class ObservableList<T> : IList<T>, INotifyCollectionChanged
    {
        private readonly List<T> _internalList = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableList(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public T this[int index] { get => _internalList[index]; set => _internalList[index] = value; }

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _internalList.Add(item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _internalList.Clear();
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(T item)
        {
            var result = _internalList.Remove(item);
            if(result)
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }

            return result;
        }

        public bool Replace(T oldItem, T newItem)
        {
            var index = _internalList.IndexOf(oldItem);
            _internalList.RemoveAt(index);
            _internalList.Insert(index, newItem);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));

            return true;
        }

        public void RemoveAt(int index)
        {
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _internalList[index]));
            _internalList.RemoveAt(index);
        }

        public void AddRange(IEnumerable<T> items)
        {
            var itemsToAdd = items.ToList();
            var count = itemsToAdd.Count;
            _internalList.AddRange(itemsToAdd);

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsToAdd, count));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}
