using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonCardCatalogue.Constants
{
    public static class ErrorMessages
    {
        public static class SetList
        {
            public const string CollectionViewWebRequestTimeoutMessage = "Oops! Looks like there was an issue finding these cards.";
        }

        public static class AllSets
        {
            public const string CollectionViewWebRequestTimeoutMessage = "Oops! Looks like there was an issue fetching the sets.";
        }

        public static class CollectionSets
        {
            public const string CollectionViewNoSetsInCollectionMessage = "Looks like you haven't added any cards to your collection. \nLet's fix that!";
        }
    }
}
