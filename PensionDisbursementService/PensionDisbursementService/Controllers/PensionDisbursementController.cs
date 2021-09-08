using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionDisbursementService.Constants;
using PensionDisbursementService.Models;
using PensionDisbursementService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionDisbursementService.Controllers
{
    [Route("api/pensionDisbursement")]
    [ApiController]
    public class PensionDisbursementController : Controller
    {
        private readonly IPensionerDetailRespository _repo;
        private readonly ILogger<PensionDisbursementController> _logger;
        public PensionDisbursementController(IPensionerDetailRespository repo, ILogger<PensionDisbursementController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("disbursePension")]
        public async Task<IActionResult> DisbursePension(ProcessPensionInput processPensionInput)
        {
            if (processPensionInput == null)
                return BadRequest();

            _logger.LogInformation($"POST: /disbursePension for {processPensionInput.AadharNumber}");

            var pensionerDetail = await _repo.GetPensionerDetailByAadhar(processPensionInput.AadharNumber);
            if (pensionerDetail == null)
            {
                _logger.LogInformation($"Unable to fetch Pensioner details for '{processPensionInput.AadharNumber}'");
                return BadRequest("Unable to fetch Pensioner detail.");
            }

            // Validate ProcessRequest with Actual Pensioner Details
            bool validInput = ValidatePension(processPensionInput, pensionerDetail);

            int processPensionStatusCode = validInput ? 10 : 21;

            ProcessPensionResponse processPensionResponse = new ProcessPensionResponse { ProcessPensionStatusCode = processPensionStatusCode };

            _logger.LogInformation($"Pension process code: '{processPensionStatusCode}'.");
            return Ok(processPensionResponse);
        }

        private bool ValidatePension(ProcessPensionInput processPensionInput, PensionerDetail pensionerDetail)
        {
            if (processPensionInput.BankServiceCharge <= 0 || processPensionInput.PensionAmount <= 0)
                return false;

            // Validate Bank Charges
            double actualCharges = BankCharges.Charges[pensionerDetail.BankDetail.BankType];
            if (processPensionInput.BankServiceCharge != actualCharges)
                return false;

            // Validate Pension Amount
            double calculatedPension = CalculatePension(pensionerDetail.SalaryEarned, pensionerDetail.Allowances, pensionerDetail.PensionType);
            if (processPensionInput.PensionAmount != calculatedPension)
                return false;

            return true;
        }

        private double CalculatePension(double salaryEarned, double allowances, PensionType pensionType)
        {
            double rate = pensionType == PensionType.Self ? 0.8 : 0.5;
            return rate * salaryEarned + allowances;
        }
    }
}
