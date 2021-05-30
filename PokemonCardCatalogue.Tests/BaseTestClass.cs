using NUnit.Framework;

namespace PokemonCardCatalogue.Tests
{
    [TestFixture]
    public abstract class BaseTestFixture
    {
        [SetUp]
        public virtual void BeforeEachTest()
        {
            CreateMocks();
            SetupMocks();
            SetupData();
        }

        [TearDown]
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
