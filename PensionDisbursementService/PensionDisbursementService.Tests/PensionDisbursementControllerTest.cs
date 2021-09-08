using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PensionDisbursementService.Controllers;
using PensionDisbursementService.Models;
using PensionDisbursementService.Repository;

namespace PensionDisbursementService.Tests
{
    [TestFixture]
    public class PensionDisbursementControllerTest
    {
        private PensionDisbursementController _controller;
        private Mock<IPensionerDetailRespository> _mockPensionDetailRepo;
        private Mock<ILogger<PensionDisbursementController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPensionDetailRepo = new Mock<IPensionerDetailRespository>();
            _mockLogger = new Mock<ILogger<PensionDisbursementController>>();

            _controller = new PensionDisbursementController(_mockPensionDetailRepo.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockPensionDetailRepo = null;
            _mockLogger = null;
            _controller = null;
        }

        [Test]
        public async Task DisbursePension_ShouldReturnBadReq_WhenReqBodyIsNull()
        {
            // Act
            var actionResult = await _controller.DisbursePension(null);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DisbursePension_ShouldReturnBadReq_WhenPensionerDetailsNotFound()
        {
            // Arrange
            _mockPensionDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(It.IsAny<string>())).ReturnsAsync((PensionerDetail)null);

            // Act
            var actionResult = await _controller.DisbursePension(new ProcessPensionInput() { AadharNumber = "123412341234"} );

            // Assert
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            BadRequestObjectResult badRequestObject = (BadRequestObjectResult)actionResult;
            Assert.That(badRequestObject.Value, Is.AssignableTo<string>());
            Assert.That((string)badRequestObject.Value, Is.EqualTo("Unable to fetch Pensioner detail."));
        }

        [TestCase(100000, 500, 100000, 20000, PensionType.Self, BankType.Public)]
        [TestCase(100000, 550, 100000, 20000, PensionType.Self, BankType.Private)]
        [TestCase(70000, 500, 100000, 20000, PensionType.Family, BankType.Public)]
        [TestCase(70000, 550, 100000, 20000, PensionType.Family, BankType.Private)]
        [TestCase(130000, 550, 160000, 50000, PensionType.Family, BankType.Private)]
        [TestCase(229000, 500, 230000, 45000, PensionType.Self, BankType.Public)]
        public async Task DisbursePension_ShouldReturnOkWith10_OnValidProcessPensionRequest(double pensionAmount, double bankServiceCharge, double salaryEarned, double allowances, PensionType pensionType, BankType bankType)
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "aadhar_Number",
                PensionAmount = pensionAmount,
                BankServiceCharge = bankServiceCharge
            };
            PensionerDetail pensionerDetail = new PensionerDetail
            {
                SalaryEarned = salaryEarned,
                Allowances = allowances,
                PensionType = pensionType,
                BankDetail = new BankDetail { BankType = bankType }
            };
            _mockPensionDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(It.IsAny<string>())).ReturnsAsync(pensionerDetail);

            // Act
            var actionResult = await _controller.DisbursePension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.Value, Is.AssignableTo<ProcessPensionResponse>());
            Assert.That(((ProcessPensionResponse)okResult.Value).ProcessPensionStatusCode, Is.EqualTo(10));
        }

        [TestCase(100000, 400, 100000, 20000, PensionType.Self, BankType.Public)]
        [TestCase(100000, 550, 41211, 20000, PensionType.Self, BankType.Private)]
        [TestCase(131231, 500, 100000, 20000, PensionType.Family, BankType.Public)]
        [TestCase(710000, 550, 4121412, 20000, PensionType.Self, BankType.Private)]
        [TestCase(130000, 130, 12311, 50000, PensionType.Family, BankType.Private)]
        [TestCase(229030, 510, 230000, 45000, PensionType.Self, BankType.Public)]
        public async Task DisbursePension_ShouldReturnOkWith21_OnInvalidProcessPensionRequest(double pensionAmount, double bankServiceCharge, double salaryEarned, double allowances, PensionType pensionType, BankType bankType)
        {
            // Arrange
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "aadhar_Number",
                PensionAmount = pensionAmount,
                BankServiceCharge = bankServiceCharge
            };
            PensionerDetail pensionerDetail = new PensionerDetail
            {
                SalaryEarned = salaryEarned,
                Allowances = allowances,
                PensionType = pensionType,
                BankDetail = new BankDetail { BankType = bankType }
            };
            _mockPensionDetailRepo.Setup(_ => _.GetPensionerDetailByAadhar(It.IsAny<string>())).ReturnsAsync(pensionerDetail);

            // Act
            var actionResult = await _controller.DisbursePension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            OkObjectResult okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.Value, Is.AssignableTo<ProcessPensionResponse>());
            Assert.That(((ProcessPensionResponse)okResult.Value).ProcessPensionStatusCode, Is.EqualTo(21));
        }
    }
}
