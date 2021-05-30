using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PokemonCardCatalogue.Common.Models
{
    public class CardItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CacheId { get; set; }
        public int NormalOwnedCount { get; set; }
        public int HoloOwnedCount { get; set; }
        public int ReverseOwnedCount { get; set; }
        public Card Card { get; set; }
        public bool Owned => NormalOwnedCount > 0
            || HoloOwnedCount > 0
            || ReverseOwnedCount > 0;


        public void IncrementOwnedCount()
        {
            if (Rarity.IsHolo(Card.Rarity))
            {
                HoloOwnedCount++;
                OnPropertyChanged(nameof(HoloOwnedCount));
            }
            else
            {
                NormalOwnedCount++;
                OnPropertyChanged(nameof(NormalOwnedCount));
            }
            OnPropertyChanged(nameof(Owned));
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
