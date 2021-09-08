using NUnit.Framework;
using Moq;
using System;
using PensionDisbursementService.Repository;
using PensionDisbursementService.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using PensionDisbursementService.Models;
using Moq.Protected;
using System.Threading;
using System.Net;
using Newtonsoft.Json;

namespace PensionDisbursementService.Tests
{
    public class PensionerDetailRepositoryTest
    {
        private PensionerDetailRespository _repository;
        private Mock<PensionerDetailService> _mockPensionerDetailService;
        private Mock<ILogger<PensionerDetailRespository>> _mocklogger;
        private Mock<HttpMessageHandler> _mockhandler;

        [SetUp]
        public void Setup()
        {
            _mockhandler = new Mock<HttpMessageHandler>();
            HttpClient client = new HttpClient(_mockhandler.Object) { BaseAddress = new Uri("https://test") };
            _mockPensionerDetailService = new Mock<PensionerDetailService>(client);
            _mocklogger = new Mock<ILogger<PensionerDetailRespository>>();

            _repository = new PensionerDetailRespository(_mockPensionerDetailService.Object, _mocklogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockhandler = null;
            _mocklogger = null;
            _mockPensionerDetailService = null;
            _repository = null;
        }

        [Test]
        public async Task GetPensionerDetailByAadhar_ShouldReturnNull_OnHttpException()
        {
            // Arrange
            string aadhar = "111122223333";
            _mockhandler.Protected()
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
            _mockhandler.Protected()
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
            _mockhandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            PensionerDetail pensionerDetail = await _repository.GetPensionerDetailByAadhar(aadhar);

            // Assert
            Assert.That(pensionerDetail, Is.InstanceOf<PensionerDetail>());
        }
    }
}
