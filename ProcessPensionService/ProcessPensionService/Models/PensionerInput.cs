using System;
namespace ProcessPensionService.Models
{
    public class PensionerInput
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PAN { get; set; }
        public string AadharNumber { get; set; }
        public PensionType PensionType { get; set; }
    }
}

