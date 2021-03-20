using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Models.Collection;
using System.Collections.Generic;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface ICollectionMapper
    {
        List<SetItem> GetSetItems(List<CollectionSet> sets);
        CollectionCard GetCardCollection(Card card);
        CardItem GetCard(CollectionCard collectionItem);
        List<CardItem> GetCardList(List<CollectionCard> card);
        List<CollectionCard> GetCardCollectionList(List<Card> card);
    }
}
