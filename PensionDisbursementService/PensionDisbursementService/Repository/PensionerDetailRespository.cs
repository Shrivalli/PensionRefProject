using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PensionDisbursementService.Models;
using PensionDisbursementService.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PensionDisbursementService.Repository
{
    public class PensionerDetailRespository : IPensionerDetailRespository
    {
        private readonly HttpClient _pensionerDetailClient;
        private readonly ILogger<PensionerDetailRespository> _logger;
        public PensionerDetailRespository(PensionerDetailService pensionerDetailService, ILogger<PensionerDetailRespository> logger)
        {
            _pensionerDetailClient = pensionerDetailService.PensionerDetailClient;
            _logger = logger;
        }

        public async Task<PensionerDetail> GetPensionerDetailByAadhar(string aadhar)
        {
            HttpResponseMessage httpResponse;
            try
            {
                string url = $"api/pensionerDetail/getDetailByAadhar/{aadhar}";

                _logger.LogInformation($"[HTTP Request] GET: {_pensionerDetailClient.BaseAddress + url}");

                httpResponse = await _pensionerDetailClient.GetAsync(url);
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

            string response = await httpResponse.Content.ReadAsStringAsync();
            PensionerDetail pensionerDetail = JsonConvert.DeserializeObject<PensionerDetail>(response);
            return pensionerDetail;
        }
    }
}
