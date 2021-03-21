using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Services.Interfaces;

namespace PokemonCardCatalogue.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
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

        public CardViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
        }

        public override void Init(object parameter)
        {
            if (parameter is Card card)
            {
                ThisCard = card;
            }
        }
    }
}
