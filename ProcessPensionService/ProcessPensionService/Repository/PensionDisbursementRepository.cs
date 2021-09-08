using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessPensionService.Models;
using ProcessPensionService.Services;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPensionService.Repository
{
    public class PensionDisbursementRepository : IPensionDisbursementRepository
    {
        private readonly HttpClient _pensionDisbursementClient;
        private ILogger<PensionDisbursementRepository> _logger;

        public PensionDisbursementRepository(PensionDisbursementService pensionDisbursementService, ILogger<PensionDisbursementRepository> logger)
        {
            _pensionDisbursementClient = pensionDisbursementService.PensionDisbursementClient;
            _logger = logger;
        }

        public async Task<ProcessPensionResponse> DisbursePension(ProcessPensionInput processPensionInput)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(processPensionInput), Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                string url = "api/pensionDisbursement/disbursePension/";

                _logger.LogInformation($"[HTTP Request] Post: {url}");
                httpResponse = await _pensionDisbursementClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);

                return null;
            }

            _logger.LogInformation($"[HTTP Response] Status: {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }

            string responseString = await httpResponse.Content.ReadAsStringAsync();
            ProcessPensionResponse response = JsonConvert.DeserializeObject<ProcessPensionResponse>(responseString);
            return response;
        }
    }
}
