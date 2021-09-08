using System;
using System.Net.Http;

namespace ProcessPensionService.Services
{
    public class PensionerDetailService
    {
        public HttpClient PensionerDetailClient { get; private set; }
        public PensionerDetailService(HttpClient httpClient)
        {
            PensionerDetailClient = httpClient;
        }
    }
}
