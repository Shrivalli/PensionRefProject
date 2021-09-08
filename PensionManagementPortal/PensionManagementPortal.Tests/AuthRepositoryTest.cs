using System;
using NUnit.Framework;
using Moq;
using PensionManagementPortal.Repository;
using PensionManagementPortal.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using PensionManagementPortal.Models;
using Moq.Protected;
using System.Threading;
using Newtonsoft.Json;

namespace PensionManagementPortal.Tests
{
    [TestFixture]
    public class AuthRepositoryTest
    {
        private AuthRepository _repository;
        private Mock<AuthService> _mockAuthService;
        private Mock<ILogger<AuthRepository>> _mocklogger;
        private Mock<HttpMessageHandler> _mockhandler;

        [SetUp]
        public void Setup()
        {
            _mockhandler = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(_mockhandler.Object) { BaseAddress = new Uri("https://test") };
            _mockAuthService = new Mock<AuthService>(client);
            _mocklogger = new Mock<ILogger<AuthRepository>>();

            _repository = new AuthRepository(_mockAuthService.Object, _mocklogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockhandler = null;
            _mocklogger = null;
            _mockAuthService = null;
            _repository = null;
        }

        [Test]
        public async Task Login_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = "Username", Password = "Password" };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Moq Thrown Exception"));

            // Act
            APIResponse<AuthToken> apiResponse = await _repository.Login(credential);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<AuthToken>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Response, Is.Null);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.BadRequest)]
        public async Task Login_ShouldReturnNull_OnNonSuccessResponseStatus(HttpStatusCode statusCode)
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = "Username", Password = "Password" };

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonConvert.SerializeObject("Failure message."))
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<AuthToken> apiResponse = await _repository.Login(credential);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<AuthToken>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Message, Is.EqualTo("Failure message."));
            Assert.That(apiResponse.Response, Is.Null);
        }

        [Test]
        public async Task Logic_ShouldReturnAuthToken_OnSuccessfulAPICall()
        {
            // Arrange
            UserCredential credential = new UserCredential { UserName = "Username", Password = "Password" };
            AuthToken authToken = new AuthToken { Token = "header.payload.signature" };

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(authToken)),
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<AuthToken> apiResponse = await _repository.Login(credential);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<AuthToken>>());
            Assert.That(apiResponse.Success, Is.True);
            Assert.That(apiResponse.Message, Is.EqualTo("Login successfull!"));
            Assert.That(apiResponse.Response.Token, Is.EqualTo(authToken.Token));
        }
    }
}
