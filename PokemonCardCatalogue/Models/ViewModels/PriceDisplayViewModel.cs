using PokemonCardCatalogue.Common.Models.Data;

namespace PokemonCardCatalogue.Models
{
    public class PriceDisplayViewModel
    {
        public string Title { get; set; }
        public TcgPlayerCardRarityType Prices { get; set; }

        public PriceDisplayViewModel(string title, TcgPlayerCardRarityType prices)
        {
            this.Title = title;
            this.Prices = prices;
        }
    }
}
