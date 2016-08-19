using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.DataAccess;
using Model.Entity;
using Moq;
using SalesPlannerWeb.Controllers;
using SalesPlannerWeb.Models;

namespace SalesPlannerWeb.Tests
{
    [TestClass]
    public class LoginControllerTests
    {
        private const string ActionResponseKey = "ActionResponse";

        [TestInitialize]
        public void TestInitialize()
        {
            User.CurrentUser = null;
            WebConfiguration.Configuration = null;
        }

        [TestMethod]
        public void Login_InvalidModelSent_ReturnsIndexView()
        {
            // Arrange
            var expectedViewName = "Index";

            LoginViewModel model = new LoginViewModel();

            LoginController target = new LoginController(null, null);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            var result = target.Login(model);
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expectedViewName, viewResult.ViewName);
        }

        [TestMethod]
        public void Login_InvalidModelSent_SendsBackTheSameModel()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel();

            LoginController target = new LoginController(null, null);
            target.ModelState.AddModelError("ValidationError", "Validation error description");

            // Act
            var result = target.Login(model);
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void Login_ValidModelSent_CallsAccessorToGetUserId()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userReturnedByDb = new User { ID = "1" };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userReturnedByDb);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            userAccessorMock.Verify(mock => mock.LogIn(model.Login, model.Password));
        }

        [TestMethod]
        public void Login_UserLoggedInSuccessfully_AssignsCurrentUser()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User expected = new User
            {
                ID = "1",
                SalesOrganisationID = 1,
                LanguageCode = "en-GB"
            };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => expected);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            Assert.AreSame(expected, User.CurrentUser);
        }

        [TestMethod]
        public void Login_UserLoggedInSuccessfully_CallsClientConfigurationAccessorToGetUserConfig()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userReturnedByDb = new User { ID = "1" };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userReturnedByDb);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();
            clientConfigurationAccessorMock.Setup(mock => mock.GetClientConfiguration());

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            clientConfigurationAccessorMock.Verify(mock => mock.GetClientConfiguration());
        }

        [TestMethod]
        public void Login_GotUserConfigurationSuccessfully_AssignsUserConfigurationToWebConfiguration()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userReturnedByDb = new User { ID = "1" };

            ClientConfiguration expected = new ClientConfiguration(new List<Feature>(), new List<ROBScreen>(), new List<Screen>(), null, new Dictionary<string, string>(), new Dictionary<string, string>());

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userReturnedByDb);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();
            clientConfigurationAccessorMock.Setup(mock => mock.GetClientConfiguration()).Returns(expected);

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            Assert.AreSame(expected, WebConfiguration.Configuration);
        }

        [TestMethod]
        public void Login_CurrentUserAndHisConfigurationAssigned_ReturnsIndexViewFromListingsMgmtController()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userReturnedByDb = new User { ID = "1" };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userReturnedByDb);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            var expectedControllerName = "ListingsMgmt";
            var expectedViewName = "Index";

            // Act
            var result = target.Login(model);
            var redirectToRouteResult = result as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual(expectedControllerName, redirectToRouteResult.RouteValues.Values.ElementAt(1));
            Assert.AreEqual(expectedViewName, redirectToRouteResult.RouteValues.Values.ElementAt(0));
        }

        [TestMethod]
        public void Login_UserFailedToLogIn_DoesNotAssignCurrentUser()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userWithoutId = new User { ID = null };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userWithoutId);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            Assert.IsNull(User.CurrentUser);
        }

        [TestMethod]
        public void Login_UserFailedToLogIn_DoesNotCallClientConfigurationAccessorToGetUserConfig()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userWithoutId = new User { ID = null };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userWithoutId);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            clientConfigurationAccessorMock.Verify(x => x.GetClientConfiguration(), Times.Never);
        }

        [TestMethod]
        public void Login_UserFailedToLogIn_AssignsWarningMessageIntoTempData()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userWithoutId = new User { ID = null };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userWithoutId);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            target.Login(model);

            // Assert
            Assert.IsNotNull(target.TempData[ActionResponseKey]);

            var warningMessage = target.TempData[ActionResponseKey] as Message;
            Assert.IsNotNull(warningMessage);
            Assert.AreEqual(MessageType.Warning, warningMessage.Type);
        }

        [TestMethod]
        public void Login_UserFailedToLogIn_ReturnsIndexView()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userWithoutId = new User { ID = null };

            var expectedViewName = "Index";

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userWithoutId);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            var result = target.Login(model);
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(expectedViewName, viewResult.ViewName);
        }

        [TestMethod]
        public void Login_UserFailedToLogIn_SendsBackTheSameModel()
        {
            // Arrange
            LoginViewModel model = new LoginViewModel
            {
                Login = "admin",
                Password = "admin123"
            };

            User userWithoutId = new User { ID = null };

            var userAccessorMock = new Mock<IUser>();
            userAccessorMock.Setup(mock => mock.LogIn(model.Login, model.Password)).Returns(() => userWithoutId);

            var clientConfigurationAccessorMock = new Mock<IClientConfigurationAccess>();

            LoginController target = new LoginController(userAccessorMock.Object, clientConfigurationAccessorMock.Object);

            // Act
            var result = target.Login(model);
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreSame(model, viewResult.Model);
        }
    }
}