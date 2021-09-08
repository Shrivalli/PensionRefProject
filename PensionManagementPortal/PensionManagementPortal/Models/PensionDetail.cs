using System;
namespace PensionManagementPortal.Models
{
    public class PensionDetail
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public PensionType PensionType { get; set; }
        public double PensionAmount { get; set; }
    }
}
