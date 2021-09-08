using System;
namespace PensionerDetailService.Models
{
    public class BankDetail
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public BankType BankType { get; set; }
    }
}
