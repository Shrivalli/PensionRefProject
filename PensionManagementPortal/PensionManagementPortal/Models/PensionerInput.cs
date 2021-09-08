using System;
using System.ComponentModel.DataAnnotations;
    
namespace PensionManagementPortal.Models
{
    public class PensionerInput
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]{5}\d{4}[A-Za-z]$", ErrorMessage = "Invalid PAN")]
        public string PAN { get; set; }

        [Required]
        [Display(Name = "Aadhar Number")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Invalid Aadhar Number")]
        public string AadharNumber { get; set; }

        [Display(Name = "Pension Type")]
        public PensionType PensionType { get; set; }
    }
}
