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
using Microsoft.AspNetCore.Http;
using System.Text;

namespace PensionManagementPortal.Tests
{
    [TestFixture]
    public class ProcessPensionRepositoryTest
    {
        private ProcessPensionRepository _repository;
        private Mock<ProcessPensionService> _mockProcessPensionService;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILogger<ProcessPensionRepository>> _mocklogger;
        private Mock<HttpMessageHandler> _mockhandler;

        [SetUp]
        public void Setup()
        {
            // Mocking HttpClient
            _mockhandler = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(_mockhandler.Object) { BaseAddress = new Uri("https://test") };
            _mockProcessPensionService = new Mock<ProcessPensionService>(client);

            // Mocking Session
            byte[] token = Encoding.UTF8.GetBytes("header.payload.signature");
            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.TryGetValue("token", out token)).Returns(true);

            // Mocking HttpContext
            Mock<HttpContext> mockContext = new Mock<HttpContext>();
            mockContext.Setup(_ => _.Session).Returns(mockSession.Object);

            // Mocking HttpContextAccessor
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(mockContext.Object);

            _mocklogger = new Mock<ILogger<ProcessPensionRepository>>();

            _repository = new ProcessPensionRepository(_mockProcessPensionService.Object, _mockHttpContextAccessor.Object, _mocklogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockhandler = null;
            _mocklogger = null;
            _mockProcessPensionService = null;
            _repository = null;
        }

        // GetPensionDetail
        [Test]
        public async Task GetPensionDetail_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput();

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Moq Thrown Exception"));

            // Act
            APIResponse<PensionDetail> apiResponse = await _repository.GetPensionDetail(pensionerInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<PensionDetail>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Response, Is.Null);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.BadRequest)]
        public async Task GetPensionDetail_ShouldReturnNull_OnNonSuccessResponseStatus(HttpStatusCode statusCode)
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<PensionDetail> apiResponse = await _repository.GetPensionDetail(pensionerInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<PensionDetail>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Response, Is.Null);
        }

        [Test]
        public async Task GetPensionDetail_ShouldReturnNull_WhenNotLoggedIn()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<PensionDetail> apiResponse = await _repository.GetPensionDetail(pensionerInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<PensionDetail>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Message, Is.EqualTo("Admin not logged in."));
            Assert.That(apiResponse.Response, Is.Null);
        }

        [Test]
        public async Task GetPensionDetail_ShouldReturnPensionDetail_OnSuccessfulAPICall()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new PensionDetail()))
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<PensionDetail> apiResponse = await _repository.GetPensionDetail(pensionerInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<PensionDetail>>());
            Assert.That(apiResponse.Success, Is.True);
            Assert.That(apiResponse.Message, Is.EqualTo("Pension details fetched successfully!"));
            Assert.That(apiResponse.Response, Is.Not.Null);
        }

        [Test]
        public async Task ProcessPension_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput();

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Moq Thrown Exception"));

            // Act
            APIResponse<ProcessPensionInfo> apiResponse = await _repository.ProcessPension(processPensionInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<ProcessPensionInfo>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Response, Is.Null);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.BadRequest)]
        public async Task ProcessPension_ShouldReturnNull_OnNonSuccessResponseStatus(HttpStatusCode statusCode)
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<ProcessPensionInfo> apiResponse = await _repository.ProcessPension(processPensionInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<ProcessPensionInfo>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Response, Is.Null);
        }

        [Test]
        public async Task ProcessPension_ShouldReturnNull_WhenNotLoggedIn()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<ProcessPensionInfo> apiResponse = await _repository.ProcessPension(processPensionInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<ProcessPensionInfo>>());
            Assert.That(apiResponse.Success, Is.False);
            Assert.That(apiResponse.Message, Is.EqualTo("Admin not logged in."));
            Assert.That(apiResponse.Response, Is.Null);
        }

        // Process Pension
        [Test]
        public async Task ProcessPension_ShouldReturnPensionDetail_OnSuccessfulAPICall()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new PensionDetail()))
            };

            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            APIResponse<ProcessPensionInfo> apiResponse = await _repository.ProcessPension(processPensionInput);

            // Assert
            Assert.That(apiResponse, Is.InstanceOf<APIResponse<ProcessPensionInfo>>());
            Assert.That(apiResponse.Success, Is.True);
            Assert.That(apiResponse.Message, Is.EqualTo("Pension processed successfully!"));
            Assert.That(apiResponse.Response, Is.Not.Null);
        }
    }
}
