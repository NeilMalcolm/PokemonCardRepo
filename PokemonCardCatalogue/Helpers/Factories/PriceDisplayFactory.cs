using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using System.Collections.Generic;

namespace PokemonCardCatalogue.Helpers.Factories
{
    public static class PriceDisplayFactory
    {
        private const string HoloFoilTitle = "Holofoil";
        private const string ReverseHoloFoilTitle = "Reverse";
        private const string NormalTitle = "Normal";

        public static List<PriceDisplayViewModel> GetPriceDisplayViewModels(Price prices)
        {
            var priceModel = new List<PriceDisplayViewModel>();

            if (prices is null)
            {
                return priceModel;
            }

            if (prices.Holofoil != null)
            {
                priceModel.Add
                (
                    new PriceDisplayViewModel(HoloFoilTitle, prices.Holofoil)
                );
            }
            if (prices.ReverseHolofoil != null)
            {
                priceModel.Add
                (
                    new PriceDisplayViewModel(ReverseHoloFoilTitle, prices.ReverseHolofoil)
                );
            }
            if (prices.Normal != null)
            {
                priceModel.Add
                (
                    new PriceDisplayViewModel(NormalTitle, prices.Normal)
                );
            }

            return priceModel;
        }
    }
}
