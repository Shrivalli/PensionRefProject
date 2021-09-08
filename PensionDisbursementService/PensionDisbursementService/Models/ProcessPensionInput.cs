using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionDisbursementService.Models
{
    public class ProcessPensionInput
    {
        public string AadharNumber { get; set; }
        public double PensionAmount { get; set; }
        public double BankServiceCharge { get; set; }
    }
}