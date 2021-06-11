namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface INetworkConnectivityService
    {
        bool HasInternetConnection { get; }
    }
}
