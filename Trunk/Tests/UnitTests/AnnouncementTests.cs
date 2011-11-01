using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for Online and Offline announcments
    ///</summary>
    [TestClass()]
    public class AnnouncementTests
    {
        private static CompositionContainer _container;

        private static List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> _hello;

        private static List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> _bye;

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

            // Load messages from file
            _hello = new List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>>();
            LoadMessages(_hello, Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "\\..\\..\\Tests\\UnitTests\\AnouncementsTestHello.xml");

            _bye = new List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>>();
            LoadMessages(_bye, Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "\\..\\..\\Tests\\UnitTests\\AnouncementsTestBye.xml");
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

        //-----------------------------------------------------
        //  Helper Methods
        //-----------------------------------------------------

        #region Helper Methods

        private static void LoadMessages(List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> list, string path)
        {
            MethodInfo loadSequence = typeof(DiscoveryMessageSequence).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((x) => "ReadFrom" == x.Name);
            MethodInfo loadEndpoint = typeof(EndpointDiscoveryMetadata).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((x) => "ReadFrom" == x.Name);
            DiscoveryMessageSequenceGenerator gen = new DiscoveryMessageSequenceGenerator();

            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing("Envelope", "http://www.w3.org/2003/05/soap-envelope"))
                {
                    using (Message msg = Message.CreateMessage(reader, Int16.MaxValue, MessageVersion.Soap12))
                    {
                        EndpointDiscoveryMetadata data = new EndpointDiscoveryMetadata();

                        loadEndpoint.Invoke(data, new object[] { DiscoveryVersion.WSDiscovery11, msg.GetReaderAtBodyContents() });

                        list.Add(new Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>(gen.Next(), data));
                    }
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Online Announcement Null Argument Test
        ///</summary>
        [TestMethod()]
        public void AnnounceOnlineNullArgumentsTest()
        {
            List<Task> list = new List<Task>();

            IEnumerable<IAnounceOnlineTaskFactory> factories = _container.GetExportedValues<IAnounceOnlineTaskFactory>(ContractName.OnlineAnnouncement);
            Assert.IsNotNull(factories);
            Assert.IsTrue(0 != factories.Count());

            // Assert null arguments are handled
            try
            {
                foreach (IAnounceOnlineTaskFactory factory in factories)
                {
                    Task task = factory.Create(null, null);
                    Assert.IsNotNull(task);

                    list.Add(task);
                }

                Task.WaitAll(list.ToArray());
            }
            catch (Exception e)
            {
                if (!(e is AggregateException) || !(e.InnerException is ArgumentNullException))
                    throw;
            }
        }

        /// <summary>
        /// Offline Announcement Null Argument Test
        ///</summary>
        [TestMethod()]
        public void AnnounceOfflineNullArgumentsTest()
        {
            List<Task> list = new List<Task>();

            IEnumerable<IAnounceOfflineTaskFactory> factories = _container.GetExportedValues<IAnounceOfflineTaskFactory>(ContractName.OfflineAnnouncement);
            Assert.IsNotNull(factories);
            Assert.IsTrue(0 != factories.Count());

            // Assert null arguments are handled
            try
            {
                foreach (IAnounceOnlineTaskFactory factory in factories)
                {
                    Task task = factory.Create(null, null);
                    Assert.IsNotNull(task);

                    list.Add(task);
                }

                Task.WaitAll(list.ToArray());
            }
            catch (Exception e)
            {
                if (!(e is AggregateException) || !(e.InnerException is ArgumentNullException))
                    throw;
            }
        }

        /// <summary>
        /// Online Announcement Test
        ///</summary>
        [TestMethod()]
        public void AnnounceOnlineTests()
        {
            Assert.IsNotNull(_hello);

            IEnumerable<IAnounceOnlineTaskFactory> factories = _container.GetExportedValues<IAnounceOnlineTaskFactory>(ContractName.OnlineAnnouncement);
            Assert.IsNotNull(factories);

            foreach (var item in _hello)
            {
                foreach (var factory in factories)
                    factory.Create(new DiscoveryMessageSequence[] { item.Item1 }, new EndpointDiscoveryMetadata[] { item.Item2 });
            }
        }

        /// <summary>
        /// Offline Announcement Test
        ///</summary>
        [TestMethod()]
        public void AnnounceOfflineTests()
        {
            //Assert.IsNotNull(_hello);

            //IEnumerable<IAnounceOfflineTaskFactory> factories = _container.GetExportedValues<IAnounceOfflineTaskFactory>(ContractName.OfflineAnnouncement);
            //Assert.IsNotNull(factories);

            //foreach (var item in _)
            //{
            //    foreach (var factory in factories)
            //        factory.Create(new DiscoveryMessageSequence[] { item.Item1 }, new EndpointDiscoveryMetadata[] { item.Item2 });
            //}
        }

        /// <summary>
        /// Multiple Online Announcements Test
        ///</summary>
        [TestMethod()]
        public void AnnounceMultipleOnlineTests()
        {
            Assert.IsNotNull(_hello);

            IEnumerable<IAnounceOnlineTaskFactory> factories = _container.GetExportedValues<IAnounceOnlineTaskFactory>(ContractName.OnlineAnnouncement);
            Assert.IsNotNull(factories);

            foreach (var factory in factories)
            { 
                factory.Create(_hello.Select((t) => { return t.Item1; }).ToArray(), 
                               _hello.Select((t) => { return t.Item2; }).ToArray());
            }
        }

        /// <summary>
        /// Multiple Offline Announcements Test
        ///</summary>
        [TestMethod()]
        public void AnnounceMultipleOfflineTests()
        {
            //Assert.IsNotNull(_hello);

            //IEnumerable<IAnounceOfflineTaskFactory> factories = _container.GetExportedValues<IAnounceOfflineTaskFactory>(ContractName.OfflineAnnouncement);
            //Assert.IsNotNull(factories);

            //foreach (var factory in factories)
            //{
            //    factory.Create(_.Select((t) => { return t.Item1; }).ToArray(),
            //                   _.Select((t) => { return t.Item2; }).ToArray());
            //}
        }
    }
}
