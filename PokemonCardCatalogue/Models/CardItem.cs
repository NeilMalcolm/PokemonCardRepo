using PokemonCardCatalogue.Common.Models.Data;

namespace PokemonCardCatalogue.Models
{
    public class CardItem
    {
        public int OwnedCount { get; set; }
        public Card Card { get; set; }
        public bool Owned => OwnedCount > 0;
    }
}
