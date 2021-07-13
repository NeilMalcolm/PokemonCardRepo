namespace PokemonCardCatalogue.Services.Interfaces
{
    using System;

    public interface ILog
    {
        void Debug(string message);
        void Error(string message);
        void Exception(Exception ex);
        void Info(string message);
    }
}
