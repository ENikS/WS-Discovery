using Proxy.Service.Host;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace InstallersTests
{
    
    /// <summary>
    ///This is a test class for AppConfigInstallerTest and is intended
    ///to contain all AppConfigInstallerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AppConfigInstallerTest
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
        ///A test for AppConfigInstaller Constructor
        ///</summary>
        [TestMethod()]
        public void AppConfigInstallerConstructorTest()
        {
            AppConfigInstaller target = new AppConfigInstaller();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Commit
        ///</summary>
        [TestMethod()]
        public void CommitTest()
        {
            AppConfigInstaller target = new AppConfigInstaller(); // TODO: Initialize to an appropriate value
            IDictionary savedState = null; // TODO: Initialize to an appropriate value
            target.Commit(savedState);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Install
        ///</summary>
        [TestMethod()]
        public void InstallTest()
        {
            AppConfigInstaller target = new AppConfigInstaller(); // TODO: Initialize to an appropriate value
            IDictionary stateSaver = null; // TODO: Initialize to an appropriate value
            target.Install(stateSaver);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Rollback
        ///</summary>
        [TestMethod()]
        public void RollbackTest()
        {
            AppConfigInstaller target = new AppConfigInstaller(); // TODO: Initialize to an appropriate value
            IDictionary savedState = null; // TODO: Initialize to an appropriate value
            target.Rollback(savedState);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Uninstall
        ///</summary>
        [TestMethod()]
        public void UninstallTest()
        {
            AppConfigInstaller target = new AppConfigInstaller(); // TODO: Initialize to an appropriate value
            IDictionary savedState = null; // TODO: Initialize to an appropriate value
            target.Uninstall(savedState);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
