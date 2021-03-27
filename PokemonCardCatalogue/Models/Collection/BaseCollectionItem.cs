using SQLite;
using System;

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

        [Indexed]
        public DateTime? ModifiedDate { get; set; }

        [Indexed]
        public DateTime? CreatedDate { get; set; }
    }
}
