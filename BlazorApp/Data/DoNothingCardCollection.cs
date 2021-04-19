using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class DoNothingCardCollection : ICardCollection
    {
        public Task<int> AddCardAsync(Card card)
        {
            return Task.FromResult(0);
        }

        public Task<int> AddCardsAsync(List<Card> cards)
        {
            return Task.FromResult(0);
        }

        public Task<int> AddSetAsync(Set set)
        {
            return Task.FromResult(0);
        }

        public Task ClearAllCacheAsync()
        {
            return Task.CompletedTask;
        }

        public Task DeleteAllDataAsync()
        {
            return Task.CompletedTask;
        }

        public Task<int> DeleteSetAndCardsAsync(Set set)
        {
            return Task.FromResult(0);
        }

        public Task<int> ExecuteAsync(string commandText, params object[] parameters)
        {
            return Task.FromResult(0);
        }

        public Task<T> ExecuteScalarAsync<T>(string commandText, params object[] parameters)
        {
            return null;
        }

        public Task<CardItem> FindCardByQueryAsync(string query, params object[] parameters)
        {
            return null;
        }

        public Task<T> GetAsync<T>(string endpoint, QueryParameters parameters = null) where T : new()
        {
            return Task.FromResult(default(T));
        }

        public Task<CardItem> GetCardItemAsync(string cardId)
        {
            return null;
        }

        public Task<List<CardItem>> GetCardItemsAsync(string setId)
        {
            return Task.FromResult(new List<CardItem>());
        }

        public Task<List<SetItem>> GetSetItemsAsync(bool withOwnedCount = true)
        {
            return Task.FromResult(new List<SetItem>());
        }

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public Task<List<T>> QueryAsync<T>(string query) where T : new()
        {
            return Task.FromResult(new List<T>());
        }

        public Task<int> UpdateCardAsync(CardItem card)
        {
            return Task.FromResult(0);
        }
    }
}
