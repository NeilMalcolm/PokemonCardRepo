using PokemonCardCatalogue.Services.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Services
{
    public class AlertService : IAlertService
    {
        public Task<bool> ShowAlertAsync(string title, string message, string acceptText = "Accept", string cancelText = "No")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, acceptText, cancelText);
        }
    }
}
