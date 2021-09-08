using System;
namespace PensionManagementPortal.Models
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Response { get; set; }
    }
}
