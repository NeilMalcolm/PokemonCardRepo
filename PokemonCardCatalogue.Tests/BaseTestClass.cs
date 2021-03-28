using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PokemonCardCatalogue.Tests
{
    public abstract class BaseTestClass
    {
        public virtual void BeforeEachTest()
        {
            CreateMocks();
            SetupMocks();
            SetupData();
        }

        public virtual void AfterEachTest()
        {
        }

        protected virtual void CreateMocks()
        {
        }

        protected virtual void SetupMocks()
        {
        }

        protected virtual void SetupData()
        {

        }
    }
}
