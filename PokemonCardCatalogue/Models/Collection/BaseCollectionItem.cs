using SQLite;

namespace PokemonCardCatalogue.Models.Collection
{
    public abstract class BaseCollectionItem
    {
        [PrimaryKey]
        [AutoIncrement]
        public int CacheId { get; set; }

        [Indexed]
        public string Id { get; set; }

        [Indexed]
        public string Name { get; set; }
    }
}
