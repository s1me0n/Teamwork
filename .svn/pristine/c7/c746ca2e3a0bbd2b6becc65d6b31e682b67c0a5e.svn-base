using System.Linq;
using System.Linq.Expressions;
using SecretCommunicator.Data.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SecretCommunicator.Models.Interfaces;
using SecretCommunicator.Data.Interfaces;
using System.Collections.Generic;
using SecretCommunicator.Models.Library;
using SecretCommunicator.WebApi.Controllers;

namespace SecretCommunicator.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for BaseApiControllerTest and is intended
    ///to contain all BaseApiControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseApiControllerTest
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
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GetTestHelper<T>()
            where T : class , IIdentifier
        {
            var repository = Mock.Create<IRepository>();
            var messageToAdd = new Message()
            {
                Title = "testTitle",
                Content = "testContent",
                ChannelId = "43242342",
                CreatedDateTime = DateTime.Now.ToString(),
                Status = "add",
                Type = MessageTypes.Text
            };
            IList<Message> messageEntities = new List<Message>();
            messageEntities.Add(messageToAdd);
            Mock.Arrange<IQueryable<Message>>(() => repository.All<Message>(null))
                .Returns(messages => messageEntities.AsQueryable());

            var target = new MessageController(repository);
            var result = target.Get();
            Assert.IsTrue(messageEntities.Count() == 1);
            Assert.AreEqual(messageToAdd.Title, messageEntities.First().Title);
        }

        [TestMethod()]
        public void GetTest()
        {
            GetTestHelper<Message>();
        }


        /// <summary>
        ///A test for Get
        ///</summary>
        public void GetByIdTestHelper<T>()
            where T : class , IIdentifier
        {
            var repository = Mock.Create<IRepository>();
            var messageToAdd = new Message()
            {
                Title = "testTitle",
                Content = "testContent",
                ChannelId = "43242342",
                CreatedDateTime = DateTime.Now.ToString(),
                Status = "add",
                Type = MessageTypes.Text,
                Id = "32432434242"
            };
            IList<Message> messageEntities = new List<Message>();
            messageEntities.Add(messageToAdd);
            //var any = ;//Arg.IsAny<Expression<Func<Message, bool>>>();
            Mock.Arrange<Message>(
                () => repository.Find<Message>(m => m.Id == messageToAdd.Id, null))
                .IgnoreArguments()
                .Returns(messageToAdd)
                .MustBeCalled();

            var target = new MessageController(repository);
            var result = target.Get(messageToAdd.Id);
            Assert.AreEqual(messageToAdd.Id, result.Id);
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            GetByIdTestHelper<Message>();
        }
    }
}
