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
    public class SetListViewModel : BaseViewModel
    {
        private readonly ISetListLogic _setListLogic;

        private bool _canGoToCard = true;

        private Set _set;

        private List<Card> _cardList;
        public List<Card> CardList
        {
            get => _cardList; 
            set 
            { 
                _cardList = value;
                OnPropertyChanged();
            }
        }

        public string SetImageUrl { get; set; }

        public ICommand GoToCardCommand { get; set; }

        public SetListViewModel(ISetListLogic setListLogic,
            INavigationService navigationService)
            : base(navigationService)
        {
            _setListLogic = setListLogic;
        }

        public override void Init(object parameter)
        {
            if (parameter is Set set)
            {
                _set = set;
                Title = _set.Name;
                SetImageUrl = set.Images.Logo;
            }
        }

        protected override void SetUpCommands()
        {
            GoToCardCommand = new Command<Card>
            (
                async (card) => await GoToCardAsync(card), 
                (card) => _canGoToCard
            );
        }

        protected override async Task OnLoadAsync()
        {
            var allCards = await _setListLogic.GetAllCardsForSetAsync(_set.Id);
            await Task.Delay(1000);
            CardList = allCards;
        }

        private async Task GoToCardAsync(Card card)
        {
            _canGoToCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canGoToCard = true;
        }
    }
}
