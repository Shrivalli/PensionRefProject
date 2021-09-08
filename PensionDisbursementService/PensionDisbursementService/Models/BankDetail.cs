using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionDisbursementService.Models
{
    public class BankDetail
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public BankType BankType { get; set; }
    }
}
