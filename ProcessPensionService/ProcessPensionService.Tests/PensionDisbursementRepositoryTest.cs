using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using ProcessPensionService.Models;
using ProcessPensionService.Repository;
using ProcessPensionService.Services;

namespace ProcessPensionService.Tests
{
    [TestFixture]
    public class PensionDisbursementRepositoryTest
    {
        private PensionDisbursementRepository _repository;
        private Mock<PensionDisbursementService> _mockPensionDisbursementService;
        private Mock<ILogger<PensionDisbursementRepository>> _mocklogger;
        private Mock<HttpMessageHandler> _mockHandler;

        [SetUp]
        public void Setup()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(_mockHandler.Object) { BaseAddress = new Uri("https://test") };
            _mockPensionDisbursementService = new Mock<PensionDisbursementService>(client);
            _mocklogger = new Mock<ILogger<PensionDisbursementRepository>>();

            _repository = new PensionDisbursementRepository(_mockPensionDisbursementService.Object, _mocklogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockHandler = null;
            _mocklogger = null;
            _mockPensionDisbursementService = null;
            _repository = null;
        }

        [Test]
        public async Task DisbursePension_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "111122223333",
                PensionAmount = 12345.67,
                BankServiceCharge = 500
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Moq Thrown Exception"));

            // Act
            ProcessPensionResponse processPensionResponse = await _repository.DisbursePension(processPensionInput);

            // Assert
            Assert.That(processPensionResponse, Is.Null);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.BadRequest)]
        public async Task DisbursePension_ShouldReturnNull_OnNonSuccessResponseStatus(HttpStatusCode statusCode)
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "111122223333",
                PensionAmount = 12345.67,
                BankServiceCharge = 500
            };

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            ProcessPensionResponse processPensionResponse = await _repository.DisbursePension(processPensionInput);

            // Assert
            Assert.That(processPensionResponse, Is.Null);
        }

        [Test]
        public async Task DisbursePension_ShouldReturnPensionerDetail_OnSuccessFulAPICall()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "111122223333",
                PensionAmount = 12345.67,
                BankServiceCharge = 500
            };

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new ProcessPensionResponse { ProcessPensionStatusCode = 10 }))
            };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            ProcessPensionResponse processPensionResponse = await _repository.DisbursePension(processPensionInput);

            // Assert
            Assert.That(processPensionResponse, Is.InstanceOf<ProcessPensionResponse>());
        }
    }
}
