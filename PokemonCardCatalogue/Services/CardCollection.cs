using PokemonCardCatalogue.Common.Constants;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models.Collection;
using PokemonCardCatalogue.Services.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Services
{
    public class CardCollection : ICardCollection
    {
        private readonly ICollectionMapper _collectionMapper;

        SQLiteAsyncConnection _collectionConnection;

        private readonly string collectionDbPath
            = Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "collection.db"
            );

        public CardCollection(ICollectionMapper collectionMapper)
        {
            _collectionMapper = collectionMapper;
        }

        public async Task InitAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Db location: {collectionDbPath}");
#endif
            _collectionConnection = new SQLiteAsyncConnection(collectionDbPath);
            await _collectionConnection.CreateTableAsync<CollectionCard>();
            await _collectionConnection.CreateTableAsync<CollectionSet>();
        }

        public Task<List<SetItem>> GetSetItemsAsync(bool withOwnedCount = true)
        {
            if (!withOwnedCount)
            {
                return GetSetItemsWithoutCountAsync();
            }

            return GetSetItemsWithCountAsync();
        }

        public Task<int> AddSetAsync(Set set)
        {
            var collectionSet = new CollectionSet
            {
                Id = set.Id,
                Name = set.Name,
                Series = set.Series,
                Total = set.Total,
                PrintedTotal = set.PrintedTotal,
                SymbolImage = set.Images.Symbol,
                LogoImage = set.Images.Logo,
                ExpandedLegality = set.Legalities?.Expanded,
                UnlimitedLegality = set.Legalities?.Unlimited,
                PokemonTcgOnlineCode = set.PokemonTcgOnlineCode,
                DateAdded = DateTime.UtcNow,
                ReleaseDate = set.ReleaseDate,
                UpdatedAt = set.UpdatedAt
            };

            return _collectionConnection.InsertAsync(collectionSet);
        }

        public Task<int> AddCardAsync(Card card)
        {
            var collectionCard = _collectionMapper.GetCardCollection(card);
            return _collectionConnection.InsertOrReplaceAsync(collectionCard);
        }

        public Task<int> AddCardsAsync(List<Card> cards)
        {
            var collectionCards = _collectionMapper.GetCardCollectionList(cards);
            return _collectionConnection.InsertAllAsync(collectionCards);
        }

        public async Task<CardItem> GetCardItemAsync(string cardId)
        {
            var dbResult = await GetCardCollectionById(cardId);

            if (dbResult is null)
            {
                return null;
            }

            return _collectionMapper.GetCard(dbResult);
        }

        public async Task<List<CardItem>> GetCardItemsAsync(string setId)
        {
            var dbResults = await GetCardCollectionsBySetId(setId);

            if (dbResults is null)
            {
                return new List<CardItem>();
            }

            return _collectionMapper.GetCardList(dbResults);
        }

        public async Task<int> UpdateCardAsync(CardItem card)
        {
            var cardCollection = await GetCardCollectionById(card.Card.Id);

            cardCollection.HoloOwnedCount = card.HoloOwnedCount;
            cardCollection.NormalOwnedCount = card.NormalOwnedCount;
            cardCollection.ReverseHoloOwnedCount = card.ReverseOwnedCount;
            cardCollection.CreatedDate ??= DateTime.UtcNow;
            cardCollection.ModifiedDate ??= DateTime.UtcNow;

            return await _collectionConnection.UpdateAsync(cardCollection);
        }

        public async Task<int> DeleteSetAndCardsAsync(Set set)
        {
            int result = 0;
            // delete set
            result += await _collectionConnection.DeleteAsync<CollectionSet>(set.Id);

            // delete associated cards
            result += await _collectionConnection.ExecuteAsync(Queries.DeleteAllCardsForSet, set.Id);

            return result;
        }

        public Task<List<T>> QueryAsync<T>(string query) where T : new()
        {
            return _collectionConnection.QueryAsync<T>(query);
        }
        
        public Task<T> FindAsync<T>(string query, params object[] parameters) where T : new()
        {
            return _collectionConnection.FindWithQueryAsync<T>(query, parameters);
        }

        public async Task DeleteAllDataAsync()
        {
            await _collectionConnection.DeleteAllAsync<CollectionCard>();
            await _collectionConnection.DeleteAllAsync<CollectionSet>();
        }

        public async Task<CardItem> FindCardByQueryAsync(string query, params object[] parameters)
        {
            var dbResult = await _collectionConnection.FindWithQueryAsync<CollectionCard>
            (
                query,
                parameters
            );

            return _collectionMapper.GetCard(dbResult);
        }
        
        public async Task<IList<CardItem>> GetCardsByQueryAsync(string query, params object[] parameters)
        {
            var dbResult = await _collectionConnection.QueryAsync<CollectionCard>
            (
                query,
                parameters
            );

            return _collectionMapper.GetCardList(dbResult);
        }

        public Task<int> ExecuteAsync(string commandText, params object[] parameters)
        {
            return _collectionConnection.ExecuteAsync(commandText, parameters);
        }

        public Task<T> ExecuteScalarAsync<T>(string commandText, params object[] parameters)
        {
            return _collectionConnection.ExecuteScalarAsync<T>(commandText, parameters);
        }

        private Task<CollectionCard> GetCardCollectionById(string cardId)
        {
            return _collectionConnection.Table<CollectionCard>()
                .FirstOrDefaultAsync(x => x.Id == cardId);
        }

        private Task<List<CollectionCard>> GetCardCollectionsBySetId(string setId)
        {
            return _collectionConnection.Table<CollectionCard>()
                .Where(x => x.SetId == setId)
                .ToListAsync();
        }

        private async Task<List<SetItem>> GetSetItemsWithCountAsync()
        {
            var collectionSets =
                await _collectionConnection.QueryAsync<CollectionSet>(Queries.GetAllSetItemsWithOwnedCount);

            if (collectionSets is null)
            {
                return null;
            }

            return _collectionMapper.GetSetItems(collectionSets);
        }

        private async Task<List<SetItem>> GetSetItemsWithoutCountAsync()
        {
            var collectionSets = await _collectionConnection.Table<CollectionSet>().ToListAsync();

            if (collectionSets is null)
            {
                return null;
            }

            return _collectionMapper.GetSetItems(collectionSets);
        }
    }
}
