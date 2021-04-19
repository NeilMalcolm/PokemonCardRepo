using PokemonCardCatalogue.Common.Models.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PokemonCardCatalogue.Common.Models
{
    public class CardItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CacheId { get; set; }
        public int OwnedCount { get; set; }
        public Card Card { get; set; }
        public bool Owned => OwnedCount > 0;


        public void IncrementOwnedCount()
        {
            OwnedCount++;
            OnPropertyChanged(nameof(OwnedCount));
            OnPropertyChanged(nameof(Owned));
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
