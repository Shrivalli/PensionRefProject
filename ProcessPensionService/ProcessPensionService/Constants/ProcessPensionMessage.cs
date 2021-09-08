using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessPensionService.Constants
{
    public class ProcessPensionMessage
    {
        public static Dictionary<int, string> ProcessPensionCodeMessages = new Dictionary<int, string>
        {
            {10, "Pension disbursement successful."},
            {21, "Pension amount calculated is wrong. Please redo the calculation."}
        };
    }
}
