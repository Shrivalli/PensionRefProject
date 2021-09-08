using System;
using System.Collections.Generic;
using AuthorizationService.Models;

namespace AuthorizationService.Data
{
    public class Credentials
    {
        public static Dictionary<string, string> AdminCredentials = new Dictionary<string, string>
        {
            { "admin1", "123456" },
            { "admin2", "234567" },
            { "admin10", "101010" },
            { "admin20", "202020" },
        };
    }
}
