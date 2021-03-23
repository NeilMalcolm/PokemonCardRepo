using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private readonly ICardLogic _cardLogic;

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
        
        private List<Card> _relatedCards;
        public List<Card> RelatedCards
        {
            get => _relatedCards;
            set
            {
                _relatedCards = value;
                OnPropertyChanged();
            }
        }

        private List<PriceDisplayViewModel> _prices;
        public List<PriceDisplayViewModel> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
            }
        }

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

        public CardViewModel(INavigationService navigationService,
            ICardLogic cardLogic) 
            : base(navigationService)
        {
            _cardLogic = cardLogic;
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
        }

        private async Task GoToRelatedCard(Card card)
        {
            _canNavigatingToRelatedCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canNavigatingToRelatedCard = true;
        }

        protected override async Task OnLoadAsync()
        {
            var loadRelatedCards = LoadRelatedCardsFromSameSetAsync();
            var prices = SetPrices(ThisCard);

            await Task.WhenAll(loadRelatedCards, prices);
        }

        private async Task SetPrices(Card card)
        {
            IsLoadingPrices = true;

            try
            {
                Prices = await GetPricesAsync(card);
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
            IsLoadingRelatedCards = true;
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
