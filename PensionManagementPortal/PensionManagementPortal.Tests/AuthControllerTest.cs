using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PensionManagementPortal.Controllers;
using PensionManagementPortal.Models;
using PensionManagementPortal.Repository;
using System.Threading.Tasks;

namespace PensionManagementPortal.Tests
{
    [TestFixture]
    public class AuthControllerTest
    {
        private AuthController _controller;
        private Mock<IAuthRepository> mockAuthRepo = new Mock<IAuthRepository>();
        private Mock<ILogger<AuthController>> mockLogger = new Mock<ILogger<AuthController>>();

        [SetUp]
        public void Setup()
        {
            _controller = new AuthController(mockAuthRepo.Object, mockLogger.Object); 
        }

        [TearDown]
        public void Teardown()
        {
            _controller.Dispose();
            _controller = null;
        }

        [Test]
        public void OnGetIndex__ShouldReturnRedirectToAction()
        {
            //Act
            var actionResult = _controller.Index();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
            var redirectToActionResult = (RedirectToActionResult)actionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Login"));
        }

        [Test]
        public void OnGetLogin__ShouldReturnView__WhenNotLoggedIn()
        {
            // Arrange
            int loggedIn = 0;
            var value = new byte[]
            {
                (byte)(loggedIn >> 24),
                (byte)(0xFF & (loggedIn >> 16)),
                (byte)(0xFF & (loggedIn >> 8)),
                (byte)(0xFF & loggedIn)
            };

            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.TryGetValue("loggedIn", out value))
                .Returns(true);

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var actionResult = _controller.Login();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void OnGetLogin__ShouldReturnRedirectToAction__WhenLoggedIn()
        {
            // Arrange
            int loggedIn = 1;
            var value = new byte[]
            {
                (byte)(loggedIn >> 24),
                (byte)(0xFF & (loggedIn >> 16)),
                (byte)(0xFF & (loggedIn >> 8)),
                (byte)(0xFF & loggedIn)
            };

            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.TryGetValue("loggedIn", out value))
                .Returns(true);

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var actionResult = _controller.Login();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task OnPostLogin__ShouldReturnView__WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Username too short.");

            UserCredential userCredential = new UserCredential { UserName = "A", Password = "Password" };

            // Act
            var actionResult = await _controller.Login(userCredential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo("Please provide valid credentials."));
        }

        [Test]
        public async Task OnPostLogin__ShouldReturnView__WhenAPICallFails()
        {
            // Arrange
            UserCredential credential = new UserCredential
            {
                UserName = "Valid UserName",
                Password = "Valid Password"
            };
            APIResponse<AuthToken> apiResponse = new APIResponse<AuthToken>
            {
                Success = false,
                Message = "Failure message."
            };

            mockAuthRepo.Setup(_ => _.Login(credential)).ReturnsAsync(apiResponse);

            // Act
            var actionResult = await _controller.Login(credential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo(apiResponse.Message));
        }

        [Test]
        public async Task OnPostLogin__ShouldReturnRedirectToAction__WhenAPICallSuccess()
        {
            // Arrange
            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.Set("token", It.IsAny<byte[]>())).Verifiable();
            mockSession.Setup(_ => _.Set("loggedIn", It.IsAny<byte[]>())).Verifiable();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            UserCredential credential = new UserCredential
            {
                UserName = "Valid UserName",
                Password = "Valid Password"
            };
            APIResponse<AuthToken> apiResponse = new APIResponse<AuthToken>
            {
                Success = true,
                Message = "Login Successfull",
                Response = new AuthToken { Token = "a.long.token" }
            };

            mockAuthRepo.Setup(_ => _.Login(credential)).ReturnsAsync(apiResponse);

            // Act
            var actionResult = await _controller.Login(credential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)actionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectToActionResult.ControllerName, Is.EqualTo("Pension"));
            mockSession.Verify(_ => _.Set("token", It.IsAny<byte[]>()), Times.Once);
            mockSession.Verify(_ => _.Set("loggedIn", It.IsAny<byte[]>()), Times.Once);
        }

        [Test]
        public void OnGetLogout__ShouldReturnRedirectToAction()
        {
            // Arrange
            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.Clear()).Verifiable();

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var actionResult = _controller.Logout();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
            mockSession.Verify(_ => _.Clear(), Times.Once);
        }
    }
}