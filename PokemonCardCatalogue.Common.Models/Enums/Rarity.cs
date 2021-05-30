namespace PokemonCardCatalogue.Common.Models.Enums
{
    public static class Rarity
    {
        public const string AmazingRare = "Amazing Rare";
        public const string Common = "Common";
        public const string Legend = "LEGEND";
        public const string Promo = "Promo";
        public const string Rare = "Rare";
        public const string RareAce = "Rare ACE";
        public const string RareBreak = "Rare BREAK";
        public const string RareHolo = "Rare Holo";
        public const string RareHoloEx = "Rare Holo EX";
        public const string RareHoloGx = "Rare Holo GX";
        public const string RareHoloLevelX = "Rare Holo LV.X";
        public const string RareHoloStar = "Rare Holo Star";
        public const string RareHoloV = "Rare Holo V";
        public const string RareHoloVmax = "Rare Holo VMAX";
        public const string RarePrime = "Rare Prime";
        public const string RarePrismStar = "Rare Prism Star";
        public const string RareRainbow = "Rare Rainbow";
        public const string RareSecret = "Rare Secret";
        public const string RareShining = "Rare Shining";
        public const string RareShiny = "Rare Shiny";
        public const string RareShinyGx = "Rare Shiny GX";
        public const string RareUltra = "Rare Ultra";
        public const string Uncommon = "Uncommon";

        public static bool IsHolo(string rarity)
        {
            bool isHolo;

            switch (rarity)
            {
                case AmazingRare:
                case Legend:
                case RareAce:
                case RareBreak:
                case RareHolo:
                case RareHoloEx:
                case RareHoloGx:
                case RareHoloLevelX:
                case RareHoloStar:
                case RareHoloVmax:
                case RareHoloV:
                case RarePrime:
                case RarePrismStar:
                case RareRainbow:
                case RareSecret:
                case RareShining:
                case RareShiny:
                case RareShinyGx:
                case RareUltra:
                    isHolo = true;
                    break;
                default:
                    isHolo = false;
                    break;
            }

            return isHolo;
        }

        public static bool HasReverse(string rarity)
        {
            bool hasReverse;

            switch (rarity)
            {
                case Rare:
                case RareHolo:
                case Common:
                case Uncommon:
                    hasReverse = true;
                    break;
                default:
                    hasReverse = false;
                    break;
            }

            return hasReverse;
        }

        public static bool IsNormal(string rarity)
        {
            bool isNormal;

            switch (rarity)
            {
                case Rare:
                case Common:
                case Uncommon:
                    isNormal = true;
                    break;
                default:
                    isNormal = false;
                    break;
            }

            return isNormal;
        }
    }
}
