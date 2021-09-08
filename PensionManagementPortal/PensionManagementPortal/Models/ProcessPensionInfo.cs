using System;
namespace PensionManagementPortal.Models
{
    public class ProcessPensionInfo
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ProcessedPensionDetail Detail { get; set; }
    }
}
