using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for Online and Offline announcments
    ///</summary>
    [TestClass()]
    public class AnnouncementTests
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
            EndpointDiscoveryMetadata endpoint = new EndpointDiscoveryMetadata();

            var dd = Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "\\..\\..\\Tests\\UnitTests\\AnouncementsTestData.xml";

            try
            {
                //WriteObject(Directory.GetParent(typeof(ContractsRepositoryTest).Assembly.Location) + "test.xml", new object());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

            Debug.WriteLine("");
        }

        //public static void WriteObject(string fileName, object data)
        //{
        //    FileStream fs = new FileStream(fileName, FileMode.Create);
        //    XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            
        //    NetDataContractSerializer ser =
        //        new NetDataContractSerializer();

        //    ser.WriteObject(writer, data);
        //    writer.Close();
        //}

        //public static void ReadObject(string fileName)
        //{
        //    Console.WriteLine("Deserializing an instance of the object.");
        //    FileStream fs = new FileStream(fileName,
        //    FileMode.Open);
        //    XmlDictionaryReader reader =
        //        XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        //    NetDataContractSerializer ser = new NetDataContractSerializer();

        //    // Deserialize the data and read it from the instance.
        //    Person deserializedPerson =
        //        (Person)ser.ReadObject(reader, true);
        //    fs.Close();
        //    Console.WriteLine(String.Format("{0} {1}, ID: {2}",
        //    deserializedPerson.FirstName, deserializedPerson.LastName,
        //    deserializedPerson.ID));
        //}
    }
}
