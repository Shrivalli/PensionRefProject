using NUnit.Framework;
using Moq;
using System;
using ProcessPensionService.Controllers;
using ProcessPensionService.Repository;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProcessPensionService.Models;

namespace ProcessPensionService.Tests
{
    [TestFixture]
    class ProcessPensionControllerTest
    {
        private ProcessPensionController _controller;
        private Mock<IPensionerDetailRepository> _mockPensionerDetailRepo;
        private Mock<IPensionDisbursementRepository> _mockPensionDisbursementRepo;
        private Mock<ILogger<ProcessPensionController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPensionerDetailRepo = new Mock<IPensionerDetailRepository>();
            _mockPensionDisbursementRepo = new Mock<IPensionDisbursementRepository>();
            _mockLogger = new Mock<ILogger<ProcessPensionController>>();

            _controller = new ProcessPensionController(_mockPensionerDetailRepo.Object, _mockPensionDisbursementRepo.Object, _mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _mockLogger = null;
            _mockPensionDisbursementRepo = null;
            _mockPensionerDetailRepo = null;
            _controller = null;
        }

        [Test]
        public async Task GetPenisonDetail_ShouldReturnBadRequest_WhenInputIsNull()
        {
            // Act
            var actionResult = await _controller.GetPenisonDetail(null);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task GetPenisonDetail_ShouldReturnNotFound_WhenPensionerDetailNotFound()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput
            {
                AadharNumber = "122312231223"
            };
            _mockPensionerDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(pensionerInput.AadharNumber)).ReturnsAsync((PensionerDetail)null);

            // Act
            var actionResult = await _controller.GetPenisonDetail(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)actionResult;
            Assert.That(notFoundObjectResult.Value, Is.InstanceOf<string>());
            Assert.That((string)notFoundObjectResult.Value, Is.EqualTo("Unable to fetch pension details."));
        }

        [Test]
        public async Task GetPenisonDetail_ShouldReturnBadRequest_WhenPensionInputDoesNotMatch()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput
            {
                Name = "Name",
                AadharNumber = "122312231223",
                PAN = "ABCDE1234F",
                DateOfBirth = new DateTime(2000, 05, 06),
                PensionType = PensionType.Self
            };
            PensionerDetail pensionerDetail = new PensionerDetail
            {
                Name = "Another name",
                AadharNumber = "122312231227",
                PAN = "ABCDE1234F",
                DateOfBirth = new DateTime(2000, 05, 06),
                PensionType = PensionType.Self
            };
            _mockPensionerDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(pensionerInput.AadharNumber)).ReturnsAsync(pensionerDetail);

            // Act
            var actionResult = await _controller.GetPenisonDetail(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)actionResult;
            Assert.That(badRequestObjectResult.Value, Is.InstanceOf<string>());
            Assert.That((string)badRequestObjectResult.Value, Is.EqualTo("Invalid pensioner detail provided, please provide valid detail"));
        }

        [Test]
        public async Task GetPenisonDetail_ShouldReturnOk_WhenPensionInputMatches()
        {
            // Arrange
            PensionerInput pensionerInput = new PensionerInput
            {
                Name = "Name",
                AadharNumber = "122312231223",
                PAN = "ABCDE1234F",
                DateOfBirth = new DateTime(2000, 05, 06),
                PensionType = PensionType.Self
            };
            PensionerDetail pensionerDetail = new PensionerDetail
            {
                Name = "Name",
                AadharNumber = "122312231223",
                PAN = "ABCDE1234F",
                DateOfBirth = new DateTime(2000, 05, 06),
                PensionType = PensionType.Self,
                SalaryEarned = 100000,
                Allowances = 20000,

            };
            _mockPensionerDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(pensionerInput.AadharNumber)).ReturnsAsync(pensionerDetail);

            // Act
            var actionResult = await _controller.GetPenisonDetail(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)actionResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<PensionDetail>());
        }

        [Test]
        public async Task ProcessPension_ShouldReturnBadRequest_WhenInputIsNull()
        {
            // Act
            var actionResult = await _controller.ProcessPension(null);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task ProcessPension_ShouldReturnNotFound_WhenPensionerDetailNotFound()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "122312231223"
            };
            _mockPensionerDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(processPensionInput.AadharNumber)).ReturnsAsync((PensionerDetail)null);

            // Act
            var actionResult = await _controller.ProcessPension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
            NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)actionResult;
            Assert.That(notFoundObjectResult.Value, Is.InstanceOf<string>());
            Assert.That((string)notFoundObjectResult.Value, Is.EqualTo("Unable to fetch pensioner details."));
        }

        [Test]
        public async Task ProcessPension_ShouldReturnNotFound_WhenDisbursementFails()
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "122312231223",
                PensionAmount = 15000,
                BankServiceCharge = 500
            };
            PensionerDetail pensionerDetail = new PensionerDetail
            {
                Name = "Name",
                DateOfBirth = new DateTime(2000, 09, 21),
                AadharNumber = "122312231223",
                PAN = "ABCDE1234F",
                PensionType = PensionType.Self,
                BankDetail = new BankDetail { BankType = BankType.Public }
            };
            ProcessPensionResponse processPensionResponse = new ProcessPensionResponse { ProcessPensionStatusCode = 10 };

            _mockPensionerDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(processPensionInput.AadharNumber)).ReturnsAsync(pensionerDetail);
            _mockPensionDisbursementRepo.Setup(_ => _.DisbursePension(processPensionInput)).ReturnsAsync(processPensionResponse);

            // Act
            var actionResult = await _controller.ProcessPension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okObjectResult = (OkObjectResult)actionResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<ProcessPensionInfo>());
        }
    }
}
