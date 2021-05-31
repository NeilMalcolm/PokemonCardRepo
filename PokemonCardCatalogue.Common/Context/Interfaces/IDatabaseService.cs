using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context.Interfaces
{
    public interface IDatabaseService
    {
        void Init(string filePath, TimeSpan defaultCacheDuration);
        Task CreateTableAsync<T>() where T : new();
        Task InsertAsync<T>(T itemToInsert) where T : new();
        Task UpdateAsync<T>(T itemToUpdate) where T : new();
        Task DeleteAllAsync<T>() where T : new();
        Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predExpr) where T : new();
    }
}
