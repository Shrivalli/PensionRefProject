using System;
namespace PensionerDetailService.Models
{
    public class PensionerDetail
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public PensionType PensionType { get; set; }
        public int SalaryEarned { get; set; }
        public int Allowances { get; set; }
        public BankDetail BankDetail { get; set; }
    }
}
