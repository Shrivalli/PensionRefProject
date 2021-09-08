using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessPensionService.Models;
using ProcessPensionService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessPensionService.Repository
{
    public class PensionerDetailRepository : IPensionerDetailRepository
    {
        private readonly HttpClient _pensionerDetailService;
        private ILogger<PensionerDetailRepository> _logger;
        public PensionerDetailRepository(PensionerDetailService pensionerDetailService,  ILogger<PensionerDetailRepository> logger)
        {
            _pensionerDetailService = pensionerDetailService.PensionerDetailClient;
            _logger = logger;
        }

        public async Task<PensionerDetail> GetPensionerDetailByAadhar(string aadharNumber)
        {
            HttpResponseMessage response;
            try
            {
                string url = "api/pensionerDetail/getDetailByAadhar/" + aadharNumber;

                _logger.LogInformation($"[HTTP Request] GET: {url}");

                response = await _pensionerDetailService.GetAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);

                return null;
            }

            _logger.LogInformation($"[HTTP Response] Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string responseValue = await response.Content.ReadAsStringAsync();
            PensionerDetail pensionDetail = JsonConvert.DeserializeObject<PensionerDetail>(responseValue);
            return pensionDetail;
        }
    }
}
