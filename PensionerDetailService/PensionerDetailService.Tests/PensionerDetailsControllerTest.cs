using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PensionerDetailService.Controllers;
using PensionerDetailService.Models;
using PensionerDetailService.Repository;

namespace PensionerDetailService.Tests
{
    [TestFixture]
    public class PensionerDetailsControllerTest
    {
        private PensionerDetailsController _controller;
        private Mock<IPensionerDetailsRepository> _mockRepo;
        private Mock<ILogger<PensionerDetailsController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPensionerDetailsRepository>();
            _mockLogger = new Mock<ILogger<PensionerDetailsController>>();

            _controller = new PensionerDetailsController(_mockRepo.Object, _mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockLogger = null;
            _mockRepo = null;
            _controller = null;
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetDetailByAadhar_ShouldReturnBadRequest_WhenAadharIsNullOrEmpty(string aadhar)
        {
            // Act
            var actionResult = _controller.GetDetailByAadhar(aadhar);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)actionResult;
            Assert.That(badRequestObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)badRequestObjectResult.Value, Is.EqualTo("Aadhar number not provided."));
        }

        [TestCase("1242124")]
        [TestCase("12421241214124124")]
        [TestCase("1214GAH112H1")]
        [TestCase("ABABABABABAB")]
        public void GetDetailByAadhar_ShouldReturnBadRequest_WhenAadharIsInvalid(string aadhar)
        {
            // Act
            var actionResult = _controller.GetDetailByAadhar(aadhar);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)actionResult;
            Assert.That(badRequestObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)badRequestObjectResult.Value, Is.EqualTo("Invalid Aadhar number"));
        }

        [TestCase("876594158734")]
        [TestCase("987425434856")]
        [TestCase("183453545454")]
        [TestCase("101004726123")]
        [TestCase("633493323126")]
        public void GetDetailByAadhar_ShouldReturnNotFound_WhenAPensionerDoesNotExistForGivenAadhar(string aadhar)
        {
            _mockRepo.Setup(_ => _.GetPensionerDetailsByAadhar(aadhar)).Returns((PensionerDetail)null);
            // Act
            var actionResult = _controller.GetDetailByAadhar(aadhar);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)actionResult;
            Assert.That(notFoundObjectResult.Value, Is.AssignableTo<string>());
            Assert.That((string)notFoundObjectResult.Value, Is.EqualTo("Pensioner detail not found."));
        }

        [TestCase("876594358734")]
        [TestCase("987426734856")]
        [TestCase("187493545454")]
        [TestCase("907493726178")]
        [TestCase("638493726126")]
        public void GetDetailByAadhar_ShouldReturnOk_WhenAPensionerDetailExistForGivenAadhar(string aadhar)
        {
            _mockRepo.Setup(_ => _.GetPensionerDetailsByAadhar(aadhar)).Returns(new PensionerDetail());
            // Act
            var actionResult = _controller.GetDetailByAadhar(aadhar);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)actionResult;
            Assert.That(okObjectResult.Value, Is.AssignableTo<PensionerDetail>());
        }
    }
}
