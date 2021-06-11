using PokemonCardCatalogue.Services.Interfaces;
using Xamarin.Essentials;

namespace PokemonCardCatalogue.Services
{
    public class NetworkConnectivityService : INetworkConnectivityService
    {
        public bool HasInternetConnection => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
