using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface ICardCollection
    {
        Task InitAsync();
        Task<CardItem> GetCardItemAsync(string cardId);
        Task<List<CardItem>> GetCardItemsAsync(string setId);
        Task<int> UpdateCardAsync(CardItem card);
        Task<int> AddCardAsync(Card card);
        Task<int> AddCardsAsync(List<Card> cards);
        Task<int> AddSetAsync(Set set);
        Task<List<SetItem>> GetSetItemsAsync(bool withOwnedCount = true); 
        Task<int> DeleteSetAndCardsAsync(Set set);
        Task<List<T>> QueryAsync<T>(string query) where T : new();
        Task DeleteAllDataAsync();
        Task<CardItem> FindCardByQueryAsync(string query, params object[] parameters);
        Task<int> ExecuteAsync(string commandText, params object[] parameters);
        Task<T> ExecuteScalarAsync<T>(string commandText, params object[] parameters);
    }
}
