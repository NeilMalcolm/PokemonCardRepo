using System.Threading.Tasks;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface INavigationService
    {
        Page GetMainPage();
        Task GoToAsync<T>(object parameter = null) where T : Page;
        Task PopAsync();
    }
}
