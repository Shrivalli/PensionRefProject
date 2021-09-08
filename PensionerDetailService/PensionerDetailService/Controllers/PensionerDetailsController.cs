using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionerDetailService.Models;
using PensionerDetailService.Repository;


namespace PensionerDetailService.Controllers
{
    [Route("api/pensionerDetail")]
    [ApiController]
    public class PensionerDetailsController : ControllerBase
    {
        private readonly IPensionerDetailsRepository _repo;
        private readonly ILogger<PensionerDetailsController> _logger;

        public PensionerDetailsController(IPensionerDetailsRepository repo, ILogger<PensionerDetailsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }


        // GET: api/pensionerDetail/getDetailByAadhar
        [HttpGet("getDetailByAadhar/{aadhar}")]
        public ActionResult GetDetailByAadhar(string aadhar)
        {
            _logger.LogInformation($"GET: /getDetailByAadhar/{aadhar}");

            if (string.IsNullOrWhiteSpace(aadhar))
            {
                _logger.LogError($"Aadhar number not provided.");
                return BadRequest($"Aadhar number not provided.");
            }

            if (!ValidateAadharNumber(aadhar))
            {
                _logger.LogError($"Invalid Aadhar number.");
                return BadRequest($"Invalid Aadhar number");
            }

            PensionerDetail pensionerDetail = _repo.GetPensionerDetailsByAadhar(aadhar);

            if (pensionerDetail == null)
            {
                _logger.LogInformation($"Pensioner detail not found.");
                return NotFound("Pensioner detail not found.");
            }

            _logger.LogInformation($"Pensioner detail found.");
            return Ok(pensionerDetail);
        }

        private static bool ValidateAadharNumber(string aadhar)
        {
            Regex aadharPattern = new Regex(@"^\d{12}$");
            return aadharPattern.IsMatch(aadhar);
        }
    }
}
