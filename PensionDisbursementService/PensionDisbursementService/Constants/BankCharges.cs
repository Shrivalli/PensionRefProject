using System;
using System.Collections.Generic;
using PensionDisbursementService.Models;

namespace PensionDisbursementService.Constants
{
    public class BankCharges
    {
        public static Dictionary<BankType, double> Charges = new Dictionary<BankType, double>
        {
            { BankType.Public, 500 },
            { BankType.Private, 550 }
        };
    }
}
