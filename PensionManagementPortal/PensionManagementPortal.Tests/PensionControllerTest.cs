using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PensionManagementPortal.Controllers;
using PensionManagementPortal.Models;
using PensionManagementPortal.Repository;

namespace PensionManagementPortal.Tests
{
    [TestFixture]
    public class PensionControllerTest
    {
        private PensionController _controller;
        private Mock<IProcessPensionRepository> mockProcessPensionRepo = new Mock<IProcessPensionRepository>();
        private Mock<IPensionDbRepository> mockPensionDbRepo = new Mock<IPensionDbRepository>();
        private Mock<ILogger<PensionController>> mockLogger = new Mock<ILogger<PensionController>>();

        [SetUp]
        public void Setup()
        {
            _controller = new PensionController(mockProcessPensionRepo.Object, mockPensionDbRepo.Object, mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _controller.Dispose();
            _controller = null;
        }

        [Test]
        public void OnGetIndex__ShouldReturnRedirectToAction__WhenNotLoggedIn()
        {
            // Arrange
            MockLoggedInState(false);

            // Act
            var actionResult = _controller.Index();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)actionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Login"));
            Assert.That(redirectToActionResult.ControllerName, Is.EqualTo("Auth"));
        }

        [Test]
        public void OnGetIndex__ShouldReturnView__WhenLoggedIn()
        {
            // Arrange
            MockLoggedInState(true);

            // Act
            var actionResult = _controller.Index();

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
        }

        [Test]
        public async Task OnPostIndex__ShouldReturnViewWithErrorMsg__WhenModelInvalid()
        {
            // Arrange
            MockLoggedInState(true);
            PensionerInput pensionerInput = new PensionerInput
            {
                AadharNumber = "123",
                PAN = "MNBHG4123H",
                DateOfBirth = new DateTime(1998, 1, 3),
                Name = "Name",
                PensionType = PensionType.Self
            };
            _controller.ModelState.AddModelError("AadharNumber", "Invalid Aadhar Number");

            // Act
            var actionResult = await _controller.Index(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo("Please provide valid pensioner details."));
        }

        [Test]
        public async Task OnPostIndex__ShouldReturnViewWithErrorMsg__WhenAPICallFails()
        {
            // Arrange
            MockLoggedInState(true);
            PensionerInput pensionerInput = new PensionerInput
            {
                AadharNumber = "123412341234",
                PAN = "MNBHG4123H",
                DateOfBirth = new DateTime(1998, 1, 3),
                Name = "Name",
                PensionType = PensionType.Self
            };
            APIResponse<PensionDetail> apiResponse = new APIResponse<PensionDetail>
            {
                Success = false,
                Message = "Failure message"
            };
            mockProcessPensionRepo.Setup(_ => _.GetPensionDetail(pensionerInput)).ReturnsAsync(apiResponse);

            // Act
            var actionResult = await _controller.Index(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo(apiResponse.Message));
        }

        [Test]
        public async Task OnPostIndex__ShouldReturnPensionDetailView__WhenAPICallSuccessful()
        {
            // Arrange
            MockLoggedInState(true);
            PensionerInput pensionerInput = new PensionerInput
            {
                AadharNumber = "123412341234",
                PAN = "MNBHG4123H",
                DateOfBirth = new DateTime(1998, 1, 3),
                Name = "Name",
                PensionType = PensionType.Self
            };
            APIResponse<PensionDetail> apiResponse = new APIResponse<PensionDetail>
            {
                Success = true,
                Message = "Success message",
                Response = new PensionDetail()
            };
            mockProcessPensionRepo.Setup(_ => _.GetPensionDetail(pensionerInput)).ReturnsAsync(apiResponse);

            // Act
            var actionResult = await _controller.Index(pensionerInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewName, Is.EqualTo("PensionDetail"));
            Assert.That(viewResult.Model, Is.InstanceOf<PensionDetailViewModel>());

        }

        [Test]
        public async Task OnPostProcessPension__ShouldReturnIndexView__WhenAPICallFails()
        {
            // Arrange
            MockLoggedInState(true);
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "123412341234",
                PensionAmount = 1234.56,
                BankServiceCharge = 500
            };
            APIResponse<ProcessPensionInfo> apiResponse = new APIResponse<ProcessPensionInfo>
            {
                Success = false,
                Message = "Failure message"
            };
            mockProcessPensionRepo.Setup(_ => _.ProcessPension(processPensionInput)).ReturnsAsync(apiResponse);

            // Act
            var actionResult = await _controller.ProcessPension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewData["ErrorMessage"], Is.EqualTo(apiResponse.Message));
        }

        [Test]
        public async Task OnPostProcessPension__ShouldReturnPensionProcessStatusView__WhenAPICallSuccessful()
        {
            // Arrange
            MockLoggedInState(true);
            ProcessPensionInput processPensionInput = new ProcessPensionInput
            {
                AadharNumber = "123412341234",
                PensionAmount = 1234.56,
                BankServiceCharge = 500
            };
            APIResponse<ProcessPensionInfo> apiResponse = new APIResponse<ProcessPensionInfo>
            {
                Success = true,
                Message = "Success message",
                Response = new ProcessPensionInfo
                {
                    Detail = new ProcessedPensionDetail()
                }
            };
            mockProcessPensionRepo.Setup(_ => _.ProcessPension(processPensionInput)).ReturnsAsync(apiResponse);
            mockPensionDbRepo.Setup(_ => _.AddProcessedPensionDetail(apiResponse.Response.Detail)).Verifiable();

            // Act
            var actionResult = await _controller.ProcessPension(processPensionInput);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.That(viewResult.ViewName, Is.EqualTo("PensionProcessStatus"));
            Assert.That(viewResult.Model, Is.InstanceOf<ProcessPensionInfo>());
        }

        private void MockLoggedInState(bool loggedIn)
        {
            // Arrange
            byte[] value = new byte[] { 0, 0, 0, loggedIn ? (byte)1 : (byte)0 };

            Mock<ISession> mockSession = new Mock<ISession>();
            mockSession.Setup(_ => _.TryGetValue("loggedIn", out value)).Returns(true);

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }
    }
}