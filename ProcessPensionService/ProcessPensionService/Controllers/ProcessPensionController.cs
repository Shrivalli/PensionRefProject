using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessPensionService.Constants;
using ProcessPensionService.Models;
using ProcessPensionService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessPensionService.Controllers
{

    [Route("api/processPension")]
    [ApiController]
    [Authorize]
    public class ProcessPensionController : ControllerBase
    {
        private IPensionerDetailRepository _pensionerDetailRepo;
        private IPensionDisbursementRepository _pensionDisbursementRepo;
        private ILogger<ProcessPensionController> _logger;
        public ProcessPensionController(IPensionerDetailRepository pensionerDetailRepo, IPensionDisbursementRepository pensionDisbursementRepo, ILogger<ProcessPensionController> logger)
        {
            _pensionerDetailRepo = pensionerDetailRepo;
            _pensionDisbursementRepo = pensionDisbursementRepo;
            _logger = logger;
        }


        [HttpPost("getPensionDetail")]
        public async Task<IActionResult> GetPenisonDetail(PensionerInput pensionerInput)
        {
            if (pensionerInput == null)
                return BadRequest();

            _logger.LogInformation($"POST: /getPensionDetail for '{pensionerInput.AadharNumber}'");

            PensionerDetail pensionerDetail = await _pensionerDetailRepo.GetPensionerDetailByAadhar(pensionerInput.AadharNumber);
            if (pensionerDetail == null)
            {
                return NotFound("Unable to fetch pension details.");
            }

            if (!ValidatePensionerDetail(pensionerInput, pensionerDetail))
            {
                _logger.LogInformation($"Details did not match for '{pensionerInput.AadharNumber}'");
                return BadRequest("Invalid pensioner detail provided, please provide valid detail");
            }

            double pensionAmount = CalculatePension(pensionerDetail.SalaryEarned, pensionerDetail.Allowances, pensionerDetail.PensionType);

            PensionDetail pensionDetail = new PensionDetail
            {
                Name = pensionerDetail.Name,
                DateOfBirth = pensionerDetail.DateOfBirth,
                PAN = pensionerDetail.PAN,
                AadharNumber = pensionerDetail.AadharNumber,
                PensionType = pensionerDetail.PensionType,
                PensionAmount = pensionAmount
            };

            _logger.LogInformation($"Pension details found and calculated for '{pensionerInput.AadharNumber}'");
            return Ok(pensionDetail);
        }

        [HttpPost("processPension")]
        public async Task<IActionResult> ProcessPension(ProcessPensionInput processPensionInput)
        {
            if (processPensionInput == null)
                return BadRequest();
            
            PensionerDetail pensionerDetail = await _pensionerDetailRepo.GetPensionerDetailByAadhar(processPensionInput.AadharNumber);
            if (pensionerDetail == null)
            {
                return NotFound("Unable to fetch pensioner details.");
            }

            // processPensionInput.BankServiceCharge = BankServiceCharge.Charges[pensionerDetail.BankDetail.BankType];

            ProcessPensionResponse processCode = await _pensionDisbursementRepo.DisbursePension(processPensionInput);
            if (processCode == null)
            {
                return NotFound("Unable to process the pension.");
            }

            ProcessPensionInfo processPensionInfo = new ProcessPensionInfo
            {
                Success = processCode.ProcessPensionStatusCode == 10,
                Message = ProcessPensionMessage.ProcessPensionCodeMessages[processCode.ProcessPensionStatusCode],
                Detail = new ProcessedPensionDetail
                {
                    PensionerName = pensionerDetail.Name,
                    DateOfBirth = pensionerDetail.DateOfBirth,
                    AadharNumber = pensionerDetail.AadharNumber,
                    PAN = pensionerDetail.AadharNumber,
                    PensionType = pensionerDetail.PensionType.ToString(),
                    BankType = pensionerDetail.BankDetail.BankType.ToString(),
                    BankName = pensionerDetail.BankDetail.BankName,
                    AccountNumber = pensionerDetail.BankDetail.AccountNumber,
                    PensionAmount = processPensionInput.PensionAmount,
                    BankCharge = processPensionInput.BankServiceCharge,
                    Processed = processCode.ProcessPensionStatusCode == 10
                }
            };

            _logger.LogInformation($"Pension processed for '{processPensionInput.AadharNumber}' with {processCode.ProcessPensionStatusCode} process code.");
            return Ok(processPensionInfo);
        }


        private static bool ValidatePensionerDetail(PensionerInput pensionerInput, PensionerDetail pensionerDetail)
        {
            if (pensionerInput.Name != pensionerDetail.Name) return false;
            if (pensionerInput.DateOfBirth != pensionerDetail.DateOfBirth) return false;
            if (pensionerInput.PAN != pensionerDetail.PAN) return false;
            if (pensionerInput.AadharNumber != pensionerDetail.AadharNumber) return false;
            if (pensionerInput.PensionType != pensionerDetail.PensionType) return false;
            return true;
        }

        private static double CalculatePension(double salaryEarned, double allowances, PensionType pensionType)
        {
            double percent = pensionType == PensionType.Self ? 0.8 : 0.5;
            return salaryEarned * percent + allowances;
        }
    }
}