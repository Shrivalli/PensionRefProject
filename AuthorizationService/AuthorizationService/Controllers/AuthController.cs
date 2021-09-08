using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationService.Models;
using AuthorizationService.Repository;
using AuthorizationService.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthorizationService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IAuthRepository _repo;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthRepository repo, JwtHelper jwtHelper, ILogger<AuthController> logger)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login(UserCredential userCredential)
        {
            if (userCredential == null)
                return BadRequest();

            _logger.LogInformation($"GET: /login for '{userCredential.UserName}'");

            if (string.IsNullOrWhiteSpace(userCredential.UserName) || string.IsNullOrWhiteSpace(userCredential.Password))
            {
                _logger.LogInformation("Empty Username or Password.");

                return BadRequest("Empty Username or Password");
            }

            bool validCredential = _repo.ValidateUserCredential(userCredential);

            if (!validCredential)
            {
                _logger.LogInformation("Invalid Username or Password.");

                return NotFound("Invalid username or password");
            }

            Dictionary<string, string> claims = new Dictionary<string, string>();
            claims["username"] = userCredential.UserName;

            string token;
            try
            {
                token = _jwtHelper.GetToken(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return NotFound("Unable to process the login request. Try again later.");
            }

            AuthToken authToken = new AuthToken { Token = token };


            _logger.LogInformation($"Login Successfull for '{userCredential.UserName}'");
            return Ok(authToken);
        }
    }
}