using System.Net.Http;

namespace ProcessPensionService.Services
{
    public class PensionDisbursementService
    {
        public HttpClient PensionDisbursementClient { get; private set; }
        public PensionDisbursementService(HttpClient httpClient)
        {
            PensionDisbursementClient = httpClient;
        }
    }
}
