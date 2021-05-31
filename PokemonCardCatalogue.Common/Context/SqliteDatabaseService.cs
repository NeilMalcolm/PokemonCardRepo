using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models;
using SQLite;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context
{
    public class SqliteDatabaseService : IDatabaseService
    {
        SQLiteAsyncConnection _sqliteAsyncConnection;

        public void Init(string filePath, TimeSpan defaultCacheDuration)
        {
            _sqliteAsyncConnection = new SQLiteAsyncConnection(filePath);
        }

        public Task CreateTableAsync<T>() where T : new()
        {
            return _sqliteAsyncConnection.CreateTableAsync<T>();
        }

        public Task InsertAsync<T>(T itemToInsert) where T : new()
        {
            return _sqliteAsyncConnection.InsertAsync(itemToInsert);
        }
        

        public Task UpdateAsync<T>(T itemToUpdate) where T : new()
        {
            return _sqliteAsyncConnection.UpdateAsync(itemToUpdate);
        }
        
        public Task DeleteAllAsync<T>() where T : new()
        {
            return _sqliteAsyncConnection.DeleteAllAsync<T>();
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predExpr) where T : new()
        {
            return _sqliteAsyncConnection.Table<T>()
                .FirstOrDefaultAsync(predExpr);
        }
    }
}
