using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionManagementPortal.Models;
using PensionManagementPortal.Repository;

namespace PensionManagementPortal.Controllers
{
    public class PensionController : Controller
    {
        private readonly IProcessPensionRepository _processPensionRepo;
        private readonly IPensionDbRepository _dbRepo;
        private readonly ILogger<PensionController> _logger;

        public PensionController(IProcessPensionRepository repo, IPensionDbRepository dbRepo, ILogger<PensionController> logger)
        {
            _processPensionRepo = repo;
            _dbRepo = dbRepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"GET: /Pension/Index");

            if (!IsLoggedIn)
            {
                _logger.LogInformation($"Admin not logged in. Redirecting to Login page.");

                return RedirectToAction("Login", "Auth");
            }
            SetLoginStateInView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PensionerInput pensionerInput)
        {
            _logger.LogInformation($"POST: /Pension/Index");

            pensionerInput.Name = pensionerInput.Name.Trim();

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please provide valid pensioner details.";
                SetLoginStateInView();
                return View();
            }

            APIResponse<PensionDetail> apiResponse = await _processPensionRepo.GetPensionDetail(pensionerInput);

            if (!apiResponse.Success)
            {
                _logger.LogInformation($"{apiResponse.Message}");

                ViewBag.ErrorMessage = apiResponse.Message;
                SetLoginStateInView();
                return View();
            }

            _logger.LogInformation($"{apiResponse.Message}");

            PensionDetailViewModel viewModel = new PensionDetailViewModel
            {
                PensionDetail = apiResponse.Response
            };

            SetLoginStateInView();
            return View("PensionDetail", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPension(ProcessPensionInput processPensionInput)
        {
            _logger.LogInformation($"POST: /Pension/ProcessPension");

            APIResponse<ProcessPensionInfo> apiResponse = await _processPensionRepo.ProcessPension(processPensionInput);

            if (!apiResponse.Success)
            {
                _logger.LogInformation($"{apiResponse.Message}");

                ViewBag.ErrorMessage = apiResponse.Message;
                SetLoginStateInView();
                return View("Index");
            }

            _logger.LogInformation($"{apiResponse.Message}");

            // Saving Pension detail in Database
            await _dbRepo.AddProcessedPensionDetail(apiResponse.Response.Detail);

            SetLoginStateInView();
            return View("PensionProcessStatus", apiResponse.Response);
        }


        // Properties
        private bool IsLoggedIn { get => (HttpContext.Session.GetInt32("loggedIn") ?? 0) == 1; }

        private void SetLoginStateInView()
        {
            ViewBag.LoggedIn = IsLoggedIn;
        }
    }
}