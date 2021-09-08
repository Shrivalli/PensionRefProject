using PensionDisbursementService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionDisbursementService.Models
{
    public class PensionerDetail
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public PensionType PensionType { get; set; }
        public double SalaryEarned { get; set; }
        public double Allowances { get; set; }
        public BankDetail BankDetail { get; set; }
    }
}