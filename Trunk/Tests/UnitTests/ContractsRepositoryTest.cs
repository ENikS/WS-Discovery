using Proxy.ProbeModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for ContractsRepositoryTest and is intended
    ///to contain all ContractsRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ContractsRepositoryTest
    {
        private static CompositionContainer _container;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // Create and configure catalog
            AggregateCatalog catalog = new AggregateCatalog();

            // Load Add-in modules from the directory
            if (Directory.Exists(Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "\\Modules"))
                catalog.Catalogs.Add(new DirectoryCatalog(Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "\\Modules"));

            // Add this assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ContractsRepositoryTest).Assembly));

            // Create container
            _container = new CompositionContainer(catalog);
            if (_container == null)
                throw new InvalidOperationException();
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for ContractsRepository Constructor
        ///</summary>
        [TestMethod()]
        public void ContractsRepositoryConstructorTest()
        {
            ContractsRepository target;
            
            target = _container.GetExportedValue<ContractsRepository>();

            Assert.IsNotNull(target);
        }

    }
}
