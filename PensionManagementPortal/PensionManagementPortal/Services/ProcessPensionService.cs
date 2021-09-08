using System;
using System.Net.Http;

namespace PensionManagementPortal.Services
{
    public class ProcessPensionService
    {
        public HttpClient ProcessPensionClient { get; private set; }
        public ProcessPensionService(HttpClient httpClient)
        {
            ProcessPensionClient = httpClient;
        }
    }
}
