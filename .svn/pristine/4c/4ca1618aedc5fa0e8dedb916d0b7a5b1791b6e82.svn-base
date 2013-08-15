using SecretCommunicator.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models.Library;
using System.Net.Http;

namespace SecretCommunicator.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ChannelControllerTest and is intended
    ///to contain all ChannelControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ChannelControllerTest
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
        ///A test for Post
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://localhost:2943")]
        public void PostTest()
        {
            IRepository repository = null; // TODO: Initialize to an appropriate value
            ChannelController target = new ChannelController(repository); // TODO: Initialize to an appropriate value
            Channel value = null; // TODO: Initialize to an appropriate value
            HttpResponseMessage expected = null; // TODO: Initialize to an appropriate value
            HttpResponseMessage actual;
            actual = target.Post(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
