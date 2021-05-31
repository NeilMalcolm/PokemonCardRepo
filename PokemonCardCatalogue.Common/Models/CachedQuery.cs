using SQLite;
using System;

namespace PokemonCardCatalogue.Common.Models
{
    [Table("cached_query")]
    public class CachedQuery
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Indexed]
        public string Endpoint { get; set; }

        [Indexed]
        public string Parameters { get; set; }

        public string JsonPayload { get; set; }

        public DateTime Expiry { get; set; }

        public DateTime? LastAccessed { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
