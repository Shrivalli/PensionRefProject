using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessPensionService.Models
{
    public class ProcessPensionInfo
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ProcessedPensionDetail Detail { get; set; }
    }
}
