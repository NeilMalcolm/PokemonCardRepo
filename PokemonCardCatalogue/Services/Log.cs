namespace PokemonCardCatalogue.Services
{
    using Diagnostic = System.Diagnostics.Debug;
    using System;
    using PokemonCardCatalogue.Services.Interfaces;

    public class Log : ILog
    {
        public void Debug(string message)
        {
            Diagnostic.WriteLine(message);
        }

        public void Error(string message)
        {
            Diagnostic.WriteLine(message);
        }

        public void Exception(Exception ex)
        {
            var exception = ex;
            Diagnostic.WriteLine("+---- Exception Thrown: ----+");
            while (exception != null)
            {
                Diagnostic.WriteLine(exception.Message);
                exception = exception.InnerException;
            }
            Diagnostic.WriteLine("+---- End of Exception: ----+");
        }

        public void Info(string message)
        {
            Diagnostic.WriteLine(message);
        }
    }
}