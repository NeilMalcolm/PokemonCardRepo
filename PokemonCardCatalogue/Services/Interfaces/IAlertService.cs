using System.Threading.Tasks;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface IAlertService
    {
        Task<bool> ShowAlertAsync(string title, string message, string acceptText = "Accept", string cancelText = "No");
    }
}
