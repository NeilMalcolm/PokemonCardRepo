﻿using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface ISetListLogic
    {
        Task<List<Card>> GetAllCardsForSetAsync(string setId);
    }
}
