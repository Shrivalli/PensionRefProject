using System;
using System.ComponentModel.DataAnnotations;

namespace PensionManagementPortal.Models
{
    public class ProcessPensionInput
    {
        public string AadharNumber { get; set; }
        public double PensionAmount { get; set; }
        [Display(Name = "Bank Service Charge")]
        [Required(ErrorMessage = "Please provide bank charges as per persioner's bank type.")]
        public double BankServiceCharge { get; set; }
    }
}
