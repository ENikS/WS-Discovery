using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using Proxy.Service.Host;

namespace InstallersTests
{
    
    
    /// <summary>
    ///This is a test class for FairwallInstallerTest and is intended
    ///to contain all FairwallInstallerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FairwallInstallerTest
    {


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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
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
        ///A test for FairwallInstaller Constructor
        ///</summary>
        [TestMethod()]
        public void FairwallInstallerConstructorTest()
        {
            FairwallInstaller target = new FairwallInstaller();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WS-Discovery.Proxy.Service.exe")]
        public void DisposeTest()
        {
            FairwallInstaller_Accessor target = new FairwallInstaller_Accessor(); // TODO: Initialize to an appropriate value
            bool disposing = false; // TODO: Initialize to an appropriate value
            target.Dispose(disposing);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for InitializeComponent
        ///</summary>
        [TestMethod()]
        [DeploymentItem("WS-Discovery.Proxy.Service.exe")]
        public void InitializeComponentTest()
        {
            FairwallInstaller_Accessor target = new FairwallInstaller_Accessor(); // TODO: Initialize to an appropriate value
            target.InitializeComponent();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Install
        ///</summary>
        [TestMethod()]
        public void InstallTest()
        {
            FairwallInstaller target = new FairwallInstaller(); // TODO: Initialize to an appropriate value
            IDictionary stateSaver = null; // TODO: Initialize to an appropriate value
            target.Install(stateSaver);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Uninstall
        ///</summary>
        [TestMethod()]
        public void UninstallTest()
        {
            FairwallInstaller target = new FairwallInstaller(); // TODO: Initialize to an appropriate value
            IDictionary savedState = null; // TODO: Initialize to an appropriate value
            target.Uninstall(savedState);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
