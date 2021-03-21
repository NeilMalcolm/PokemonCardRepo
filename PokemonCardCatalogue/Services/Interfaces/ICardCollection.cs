using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Models.Collection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Services.Interfaces
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
        Task<int> SetOwnedCountForCard(string cardId, int count);
        Task<List<T>> QueryAsync<T>(string query) where T : new();
        Task DeleteAllDataAsync();
    }
}
