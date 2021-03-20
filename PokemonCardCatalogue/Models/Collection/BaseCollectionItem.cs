using SQLite;

namespace PokemonCardCatalogue.Models.Collection
{
    public abstract class BaseCollectionItem
    {
        [PrimaryKey]
        public string Id { get; set; }

        [Indexed]
        public string Name { get; set; }
    }
}
