using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.Models.Collection;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using PokemonCardCatalogue.Models;
using System.Linq;
using System;

namespace PokemonCardCatalogue.Services
{
    public class CollectionMapper : ICollectionMapper
    {
        public CollectionMapper()
        {
        }

        public List<SetItem> GetSetItems(List<CollectionSet> sets)
        {
            return Map(sets);
        }

        public List<SetItem> Map(List<CollectionSet> sets)
        {
            return sets
                .Select
                (
                    x => GetSetItem(x)
                )
                .ToList();
        }

        public CardItem GetCard(CollectionCard collectionItem)
        {
            return Map(collectionItem);
        }

        public CollectionCard GetCardCollection(Card card)
        {
            return Map(card);
        }
        
        public List<CollectionCard> GetCardCollectionList(List<Card> cards)
        {
            return Map(cards);
        }

        public List<CardItem> GetCardList(List<CollectionCard> collectionCards)
        {
            return Map(collectionCards);
        }

        private CollectionCard Map(Card card)
        {
            return GetCollectionCardFromCard(card);
        }

        private List<CollectionCard> Map(List<Card> cards)
        {
            return cards
                .Select
                (
                    x => GetCollectionCardFromCard(x)
                )
                .ToList();
        }

        private SetItem GetSetItem(CollectionSet collectionSet)
        {
            return new SetItem
            {
                OwnedCount = collectionSet.OwnedCardsCount,
                Set = new Set
                {
                    Id = collectionSet.Id,
                    Name = collectionSet.Name,
                    Images = new SetImages
                    {
                        Logo = collectionSet.LogoImage,
                        Symbol = collectionSet.SymbolImage
                    },
                    Legalities = new Legalities
                    {
                        Unlimited = collectionSet.UnlimitedLegality,
                        Expanded = collectionSet.ExpandedLegality
                    },
                    Series = collectionSet.Series,
                    PokemonTcgOnlineCode = collectionSet.PokemonTcgOnlineCode,
                    ReleaseDate = collectionSet.ReleaseDate,
                    PrintedTotal = collectionSet.PrintedTotal,
                    Total = collectionSet.Total,
                    UpdatedAt = collectionSet.UpdatedAt
                }
            };
        }

        private CardItem Map(CollectionCard cardCollection)
        {
            return GetCardItem(cardCollection);
        }

        private List<CardItem> Map(List<CollectionCard> cardCollections)
        {
            return cardCollections
                .Select
                (
                    x => GetCardItem(x)
                )
                .ToList();
        }

        private CollectionCard GetCollectionCardFromCard(Card card)
        {
            var collectionCard = new CollectionCard
            {
                AddedDate = DateTime.UtcNow,
                SetId = card.Set.Id,
                SmallImage = card.Images.Small,
                LargeImage = card.Images.Large,
                TcgPlayerUrl = card.TcgPlayer?.Url,
                Id = card.Id,
                Name = card.Name,
                Number = card.Number,
                Rarity = card.Rarity,
                Hp = card.Hp,
                ExpandedLegality = card.Legalities.Expanded,
                UnlimitedLegality = card.Legalities.Unlimited,
            };

            if (card.TcgPlayer?.Prices?.Holofoil != null)
            {
                // Holofoil
                collectionCard.HolofoilLow = card.TcgPlayer?.Prices?.Holofoil?.Low ?? float.MaxValue;
                collectionCard.HolofoilMid = card.TcgPlayer?.Prices?.Holofoil?.Mid ?? null;
                collectionCard.HolofoilHigh = card.TcgPlayer?.Prices?.Holofoil?.High ?? float.MinValue;
                collectionCard.HolofoilMarket = card.TcgPlayer?.Prices?.Holofoil?.Market ?? null;
                collectionCard.HolofoilDirectLow = card.TcgPlayer?.Prices?.Holofoil?.DirectLow ?? null;
            }

            if (card.TcgPlayer?.Prices?.Normal != null)
            {
                // Normal
                collectionCard.NormalLow = card.TcgPlayer?.Prices?.Normal?.Low ?? float.MaxValue;
                collectionCard.NormalMid = card.TcgPlayer?.Prices?.Normal?.Mid ?? null;
                collectionCard.NormalHigh = card.TcgPlayer?.Prices?.Normal?.High ?? float.MinValue;
                collectionCard.NormalMarket = card.TcgPlayer?.Prices?.Normal?.Market ?? null;
                collectionCard.NormalDirectLow = card.TcgPlayer?.Prices?.Normal?.DirectLow ?? null;
            }

            if (card.TcgPlayer?.Prices?.Normal != null)
            {
                // ReverseHolofoil
                collectionCard.ReverseHolofoilLow = card.TcgPlayer?.Prices?.ReverseHolofoil?.Low ?? float.MaxValue;
                collectionCard.ReverseHolofoilMid = card.TcgPlayer?.Prices?.ReverseHolofoil?.Mid ?? null;
                collectionCard.ReverseHolofoilHigh = card.TcgPlayer?.Prices?.ReverseHolofoil?.High ?? float.MinValue;
                collectionCard.ReverseHolofoilMarket = card.TcgPlayer?.Prices?.ReverseHolofoil?.Market ?? null;
                collectionCard.ReverseHolofoilDirectLow = card.TcgPlayer?.Prices?.ReverseHolofoil?.DirectLow ?? null;
            }

            return collectionCard;
        }

        private CardItem GetCardItem(CollectionCard collectionCard)
        {
            return new CardItem
            {
                CacheId = collectionCard.CacheId,
                OwnedCount =  collectionCard.OwnedCount,
                Card = new Card
                {
                    Id = collectionCard.Id,
                    Set = new Set
                    {
                        Id = collectionCard.SetId
                    },
                    Hp = collectionCard.Hp,
                    Name = collectionCard.Name,
                    Number = collectionCard.Number,
                    Supertype = collectionCard.Supertype,
                    ConvertedRetreatCost = collectionCard.ConvertedRetreatCost,
                    Artist = collectionCard.Artist,
                    Legalities = new Legalities
                    {
                        Expanded = collectionCard.ExpandedLegality,
                        Unlimited = collectionCard.UnlimitedLegality
                    },
                    Rarity = collectionCard.Rarity,
                    Images = new CardImages
                    {
                        Small = collectionCard.SmallImage,
                        Large = collectionCard.LargeImage
                    },
                    TcgPlayer = new TcgPlayer
                    {
                        Url = collectionCard.TcgPlayerUrl,
                        Prices = new Price
                        {
                            Holofoil = collectionCard.HolofoilHigh == null ? null : new TcgPlayerCardRarityType
                            {
                                High = collectionCard.HolofoilHigh,
                                Mid = collectionCard.HolofoilMid,
                                Low = collectionCard.HolofoilLow,
                                Market = collectionCard.HolofoilMarket,
                                DirectLow = collectionCard.HolofoilDirectLow
                            },
                            Normal = collectionCard.NormalHigh == null ? null : new TcgPlayerCardRarityType
                            {
                                High = collectionCard.NormalHigh,
                                Mid = collectionCard.NormalMid,
                                Low = collectionCard.NormalLow,
                                Market = collectionCard.NormalMarket,
                                DirectLow = collectionCard.NormalDirectLow
                            },
                            ReverseHolofoil = collectionCard.ReverseHolofoilHigh == null ? null : new TcgPlayerCardRarityType
                            {
                                High = collectionCard.ReverseHolofoilHigh,
                                Mid = collectionCard.ReverseHolofoilMid,
                                Low = collectionCard.ReverseHolofoilLow,
                                Market = collectionCard.ReverseHolofoilMarket,
                                DirectLow = collectionCard.ReverseHolofoilDirectLow
                            }
                        }
                    }
                }
            };
        }
    }
}
