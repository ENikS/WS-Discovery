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
    public partial class ProbeModuleTests
    {
        private static CompositionContainer _container;

        private static List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> _hellos;

        private static List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> _byes;

        private static List<FindRequestContext> _probes;

        private static Type _moduleType = typeof(ContractsRepository);

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
            _hellos = new List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>>();

            _probes = new List<FindRequestContext>();

            _byes = new List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>>();

            // Create and configure catalog
            AggregateCatalog catalog = new AggregateCatalog();

            // Load Add-in modules from the directory including this module
            Debug.Assert(Directory.Exists(Directory.GetParent(typeof(ProbeModuleTests).Assembly.Location).ToString()));
            catalog.Catalogs.Add(new DirectoryCatalog(Directory.GetParent(typeof(ProbeModuleTests).Assembly.Location).ToString()));

            // Create container
            _container = new CompositionContainer(catalog);
            Assert.IsNotNull(_container == null, "Failed to create MEF container");

            string dataDir = Directory.GetParent(typeof(AnnouncementTests).Assembly.Location) + "\\..\\..\\..\\Tests\\ModulesTests\\Data\\";

            // Load messages from file
            Utilities.LoadMessages(_hellos, dataDir + "TestMessagesHello.xml");
            Utilities.LoadMessages(_probes, dataDir + "TestMessagesProbe.xml");
            Utilities.LoadMessages(_byes, dataDir   + "TestMessagesBye.xml");

            // Load repository with endpoints
            foreach (var factory in _container.GetExportedValues<IAnounceOnlineTaskFactory>()
                                              .Where(x => x.GetType().Equals(_moduleType)))
            {
                factory.Create(_hellos.Select((t) => { return t.Item1; }).ToArray(),
                               _hellos.Select((t) => { return t.Item2; }).ToArray());
            }
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            foreach (var factory in _container.GetExportedValues<IAnounceOnlineTaskFactory>()
                                              .Where(x => x.GetType().Equals(_moduleType)))
            {
                factory.Create(_byes.Select((t) => { return t.Item1; }).ToArray(),
                               _byes.Select((t) => { return t.Item2; }).ToArray());
            }
        }
        
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

        /// <summary>
        /// Probe test
        ///</summary>
        [TestMethod()]
        public void ProbeTest()
        {
            IProbeTaskFactory factory = _container.GetExportedValue<IProbeTaskFactory>();
            
            Task target = factory.Create(_probes[0]);
            Assert.IsNotNull(target);

            (target as IAsyncResult).AsyncWaitHandle.WaitOne();
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof(Task<FindRequestContext>));
        }
    }
}
