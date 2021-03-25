using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Enums;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private readonly ICardLogic _cardLogic;
        private readonly ICollectionLogic _collectionLogic;
        private readonly IVibrationService _vibrationService;
        private readonly SemaphoreSlim ownedCountSemaphore = new SemaphoreSlim(1, 1);

        private bool _canNavigatingToRelatedCard = true;

        private Card _thisCard;
        public Card ThisCard
        {
            get => _thisCard;
            set
            {
                _thisCard = value;
                OnPropertyChanged();
            }
        }

        private int _ownedCount;
        public int OwnedCount
        {
            get => _ownedCount; 
            set 
            {
                _ownedCount = value;
                OnPropertyChanged();
            }
        }

        private List<Card> _relatedCards;
        public List<Card> RelatedCards
        {
            get => _relatedCards;
            set
            {
                _relatedCards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRelatedCardsSectionVisible));
            }
        }

        public bool IsRelatedCardsSectionVisible => RelatedCards?.Count > 0;

        private List<PriceDisplayViewModel> _prices;
        public List<PriceDisplayViewModel> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPricesSectionVisible));
            }
        }

        public bool IsPricesSectionVisible => Prices?.Count > 0;

        private bool _isLoadingRelatedCards;

        public bool IsLoadingRelatedCards
        {
            get => _isLoadingRelatedCards;
            set
            {
                _isLoadingRelatedCards = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isLoadingPrices;

        public bool IsLoadingPrices
        {
            get => _isLoadingPrices;
            set
            {
                _isLoadingPrices = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToRelatedCardCommand { get; set; }
        public ICommand DecrementOwnedCountCommand { get; set; }
        public ICommand IncrementOwnedCountCommand { get; set; }

        public CardViewModel(INavigationService navigationService,
            ICardLogic cardLogic,
            ICollectionLogic collectionLogic,
            IVibrationService vibrationService) 
            : base(navigationService)
        {
            _cardLogic = cardLogic;
            _collectionLogic = collectionLogic;
            _vibrationService = vibrationService;
        }

        public override void Init(object parameter)
        {
            if (parameter is Card card)
            {
                ThisCard = card;
            }
        }

        protected override void SetUpCommands()
        {
            GoToRelatedCardCommand = new Command<Card>
            (
                async (card) => await GoToRelatedCard(card), 
                (card) => _canNavigatingToRelatedCard
            );

            DecrementOwnedCountCommand = new Command
            (
                async () => await DecrementOwnedCount()
            );

            IncrementOwnedCountCommand = new Command
            (
                async () => await IncrementOwnedCount()
            );
        }

        private async Task DecrementOwnedCount()
        {
            try
            {
                OwnedCount--;
                await ownedCountSemaphore.WaitAsync();
                _vibrationService.PerformNotificationFeedbackVibration(VibrationNotificationType.Success);
                await _collectionLogic.DecrementCardOwnedCount(ThisCard.Id);
            }
            finally
            {
                ownedCountSemaphore.Release();
            }
        }

        private async Task IncrementOwnedCount()
        {
            try
            {
                OwnedCount++;
                await ownedCountSemaphore.WaitAsync();
                _vibrationService.PerformSelectionFeedbackVibration();
                await _collectionLogic.IncrementCardOwnedCount(ThisCard.Id);
            }
            finally
            {
                ownedCountSemaphore.Release();
            }
        }

        private async Task GoToRelatedCard(Card card)
        {
            _canNavigatingToRelatedCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canNavigatingToRelatedCard = true;
        }

        protected override async Task OnLoadAsync()
        {
            IsLoadingRelatedCards = true;
            IsLoadingPrices = true;
            var loadRelatedCardsTask = LoadRelatedCardsFromSameSetAsync();
            var getOwnedCountTask = GetOwnedCountAsync(ThisCard.Id);
            var pricesTask = SetPrices(ThisCard);

            await Task.WhenAll(getOwnedCountTask, loadRelatedCardsTask, pricesTask);
        }

        private async Task GetOwnedCountAsync(string id)
        {
            OwnedCount = await _collectionLogic.GetCardOwnedCount(id);
        }

        private async Task SetPrices(Card card)
        {
            try
            {
                var cards = await GetPricesAsync(card);
                await Task.Delay(800);
                Prices = cards;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoadingPrices = false;
            }
        }

        private Task<List<PriceDisplayViewModel>> GetPricesAsync(Card card)
        {
            var priceModel = new List<PriceDisplayViewModel>();

            var allTypes = new Tuple<string, TcgPlayerCardRarityType>[]
            {
                new Tuple<string, TcgPlayerCardRarityType>
                (
                    "Holofoil",
                    card.TcgPlayer.Prices.Holofoil
                ),
                new Tuple<string, TcgPlayerCardRarityType>
                (
                    "Reverse Holofoil", 
                    card.TcgPlayer.Prices.ReverseHolofoil
                ),
                new Tuple<string, TcgPlayerCardRarityType>
                (
                    "Normal",
                    card.TcgPlayer.Prices.Normal
                )
            };

            foreach (var type in allTypes)
            {
                if (type.Item2 is null)
                {
                    continue;
                }

                var cardType = type.Item2;

                priceModel.Add(new PriceDisplayViewModel
                {
                    Title = type.Item1,
                    HighPrice = cardType.High,
                    MidPrice = cardType.Mid,
                    LowPrice = cardType.Low,
                    MarketPrice = cardType.Market,
                    DirectionPrice = cardType.DirectLow
                });
            }

            return Task.FromResult(priceModel);
        }

        private async Task LoadRelatedCardsFromSameSetAsync()
        {
            try
            {
                var results = await _cardLogic.GetRelatedCardsInSetAsync(_thisCard);
                await Task.Delay(300);
                RelatedCards = results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoadingRelatedCards = false;
            }
        }
    }

    public class PriceDisplayViewModel
    {
        public string Title { get; set; }
        public float? LowPrice { get; set; }
        public float? MidPrice { get; set; }
        public float? HighPrice { get; set; }
        public float? MarketPrice { get; set; }
        public float? DirectionPrice { get; set; }
    }
}
