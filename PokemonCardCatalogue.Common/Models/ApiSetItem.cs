using PokemonCardCatalogue.Common.Models.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PokemonCardCatalogue.Common.Models
{
    public class ApiSetItem : INotifyPropertyChanged
    {
        private bool _isInCollection;
        public bool IsInCollection 
        {
            get => _isInCollection;
            set
            {
                if (value != _isInCollection)
                {
                    _isInCollection = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isDownloading;
        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                if (value != _isDownloading)
                {
                    _isDownloading = value;
                    OnPropertyChanged();
                }
            }
        }

        public Set Set { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
