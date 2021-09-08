using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessPensionService.Models
{
    public class ProcessedPensionDetail
    {
        public string PensionerName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AadharNumber { get; set; }
        public string PAN { get; set; }
        public string PensionType { get; set; }
        public string BankType { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public double PensionAmount { get; set; }
        public double BankCharge { get; set; }
        public bool Processed { get; set; }
    }
}
