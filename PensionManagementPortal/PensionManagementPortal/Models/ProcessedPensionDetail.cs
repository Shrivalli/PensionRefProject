using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionManagementPortal.Models
{
    public class ProcessedPensionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string PensionerName { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [MaxLength(15)]
        public string AadharNumber { get; set; }
        [Required]
        [MaxLength(15)]
        public string PAN { get; set; }
        [Required]
        public string PensionType { get; set; }
        [Required]
        public string BankType { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        public double PensionAmount { get; set; }
        public double BankCharge { get; set; }
        public bool Processed { get; set; }
    }
}
