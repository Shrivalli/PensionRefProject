using System;
using System.Collections.Generic;
using AuthorizationService.Controllers;
using AuthorizationService.Models;
using AuthorizationService.Repository;
using AuthorizationService.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AuthorizationService.Tests
{
    [TestFixture]
    public class AuthControllerTest
    {
        private AuthController _controller;
        private Mock<IAuthRepository> _mockAuthRepo;
        private Mock<JwtHelper> _mockJwtHelper;
        private Mock<ILogger<AuthController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var mockConfig = new Mock<IConfiguration>();
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockJwtHelper = new Mock<JwtHelper>(mockConfig.Object);
            _mockLogger = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(_mockAuthRepo.Object, _mockJwtHelper.Object, _mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockLogger = null;
            _mockJwtHelper = null;
            _mockAuthRepo = null;
            _controller = null;
        }

        [Test]
        public void GetLogin_ShouldReturnBadReq_WhenReqBodyIsNull()
        {
            // Act
            var actionResult = _controller.Login(null);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [TestCase("", "password")]
        [TestCase(null, "password")]
        [TestCase("username", "")]
        [TestCase("username", null)]
        public void GetLogin_ShouldReturnBadReqWithMsg_WhenUsernameOrPasswordEmpty(string username, string password)
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = username, Password = password };

            // Act
            var actionResult = _controller.Login(credential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)actionResult;
            Assert.That(badRequestObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)badRequestObjectResult.Value, Is.EqualTo("Empty Username or Password"));
        }

        [Test]
        public void GetLogin_ShouldReturnNotFound_OnInvalidCredential()
        {
            // Arrange
            UserCredential invalidCredential = new UserCredential { UserName = "invalid", Password = "invalid" };
            _mockAuthRepo.Setup(_ => _.ValidateUserCredential(invalidCredential)).Returns(false);

            // Act
            var actionResult = _controller.Login(invalidCredential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)actionResult;
            Assert.That(notFoundObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)notFoundObjectResult.Value, Is.EqualTo("Invalid username or password"));
        }

        [Test]
        public void GetLogin_ShouldReturnNotFound_OnTokenGenerationException()
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = "username", Password = "password" };
            _mockAuthRepo.Setup(_ => _.ValidateUserCredential(credential)).Returns(true);

            _mockJwtHelper.Setup(_ => _.GetToken(It.IsAny<Dictionary<string, string>>())).Throws(new Exception());

            // Act
            var actionResult = _controller.Login(credential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)actionResult;
            Assert.That(notFoundObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)notFoundObjectResult.Value, Is.EqualTo("Unable to process the login request. Try again later."));
        }

        [Test]
        public void GetLogin_ShouldReturnOkResult_OnValidCredential()
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = "username", Password = "password" };
            _mockAuthRepo.Setup(_ => _.ValidateUserCredential(credential)).Returns(true);

            Dictionary<string, string> claimsDict = new Dictionary<string, string> { { "username", credential.UserName} };
            _mockJwtHelper.Setup(_ => _.GetToken(claimsDict)).Returns("t.ok.en");

            // Act
            var actionResult = _controller.Login(credential);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)actionResult;
            Assert.That(okObjectResult.Value, Is.AssignableTo<AuthToken>());
            Assert.That(((AuthToken)okObjectResult.Value).Token, Is.InstanceOf<string>());
        }
    }
}
