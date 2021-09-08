using System;
using NUnit.Framework;
using Moq;
using ProcessPensionService.Repository;
using Microsoft.Extensions.Logging;
using ProcessPensionService.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;
using ProcessPensionService.Models;
using System.Net;
using Newtonsoft.Json;

namespace ProcessPensionService.Tests
{
    [TestFixture]
    public class PensionerDetailRepositoryTest
    {
        private PensionerDetailRepository _repository;
        private Mock<PensionerDetailService> _mockPensionerDetailService;
        private Mock<ILogger<PensionerDetailRepository>> _mocklogger;
        private Mock<HttpMessageHandler> _mockHandler;

        [SetUp]
        public void Setup()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(_mockHandler.Object) { BaseAddress = new Uri("https://test") };
            _mockPensionerDetailService = new Mock<PensionerDetailService>(client);
            _mocklogger = new Mock<ILogger<PensionerDetailRepository>>();

            _repository = new PensionerDetailRepository(_mockPensionerDetailService.Object, _mocklogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockHandler = null;
            _mocklogger = null;
            _mockPensionerDetailService = null;
            _repository = null;
        }

        [Test]
        public async Task GetPensionerDetailByAadhar_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            string aadhar = "111122223333";
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Moq Thrown Exception"));

            // Act
            PensionerDetail pensionerDetail = await _repository.GetPensionerDetailByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.Null);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.BadRequest)]
        public async Task GetPensionerDetailByAadhar_ShouldReturnNull_OnNonSuccessResponseStatus(HttpStatusCode statusCode)
        {
            // Arrange
            string aadhar = "111122223333";
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            PensionerDetail pensionerDetail = await _repository.GetPensionerDetailByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.Null);
        }

        [Test]
        public async Task GetPensionerDetailByAadhar_ShouldReturnPensionerDetail_OnSuccessFulAPICall()
        {
            // Arrange
            string aadhar = "111122223333";
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new PensionerDetail { AadharNumber = aadhar }))
            };
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            PensionerDetail pensionerDetail = await _repository.GetPensionerDetailByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.InstanceOf<PensionerDetail>());
        }
    }
}
