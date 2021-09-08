using ProcessPensionService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessPensionService.Controllers
{
    public class BankServiceCharge
    {
        public static readonly Dictionary<BankType, int> Charges = new Dictionary<BankType, int>
        {
            { BankType.Public, 500},
            { BankType.Private, 550}
        };
    }
}