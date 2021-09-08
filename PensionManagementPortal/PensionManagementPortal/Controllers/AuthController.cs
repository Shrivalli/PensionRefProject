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
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository authRepo, ILogger<AuthController> logger)
        {
            _authRepo = authRepo;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            _logger.LogInformation("GET: /Auth/Login");

            if (IsLoggedIn)
            {
                return RedirectToAction("Index", "Pension");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserCredential credential)
        {
            _logger.LogInformation($"POST: /Auth/Login for {credential.UserName}");

            credential.UserName = credential.UserName.Trim();
            credential.Password = credential.Password.Trim();

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please provide valid credentials.";
                return View();
            }

            APIResponse<AuthToken> apiResponse = await _authRepo.Login(credential);

            if(!apiResponse.Success)
            {
                _logger.LogInformation($"Login failed. Message: {apiResponse.Message}");

                ViewBag.ErrorMessage = apiResponse.Message;
                return View();
            }

            _logger.LogInformation($"{apiResponse.Message}");

            // Store the token in session and set login status.
            Token = apiResponse.Response.Token;
            IsLoggedIn = true;

            return RedirectToAction("Index", "Pension");
        }


        public IActionResult Logout()
        {
            _logger.LogInformation($"GET: Logout");

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        // Properties
        private string Token { get => HttpContext.Session.GetString("token"); set => HttpContext.Session.SetString("token", value); }
        private bool IsLoggedIn { get => (HttpContext.Session.GetInt32("loggedIn") ?? 0) == 1; set => HttpContext.Session.SetInt32("loggedIn", value ? 1 : 0); }
        //private string LoggedInUser { get => HttpContext.Session.GetString("user"); set => HttpContext.Session.SetString("user", value); }
    }
}