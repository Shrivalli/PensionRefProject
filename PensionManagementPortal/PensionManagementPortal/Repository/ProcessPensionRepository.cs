using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PensionManagementPortal.Models;
using PensionManagementPortal.Services;

namespace PensionManagementPortal.Repository
{
    public class ProcessPensionRepository : IProcessPensionRepository
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly HttpClient _processPensionClient;
        private readonly ILogger<ProcessPensionRepository> _logger;

        private string Token { get => _httpContext.HttpContext.Session.GetString("token"); }

        public ProcessPensionRepository(ProcessPensionService processPensionService, IHttpContextAccessor httpContext, ILogger<ProcessPensionRepository> logger)
        {
            _processPensionClient = processPensionService.ProcessPensionClient;
            _httpContext = httpContext;
            _logger = logger;
        }

        public async Task<APIResponse<PensionDetail>> GetPensionDetail(PensionerInput pensionerInput)
        {
            _processPensionClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Token}");

            StringContent requestContent = new StringContent(JsonSerializer.Serialize(pensionerInput), Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                string url = "/api/processPension/getPensionDetail";
                _logger.LogInformation($"[HTTP Request] POST : {_processPensionClient.BaseAddress + url}");

                httpResponse = await _processPensionClient.PostAsync("api/processPension/getPensionDetail", requestContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<PensionDetail>
                {
                    Message = "Something went wrong. Try again later"
                };
            }

            _logger.LogInformation($"[HTTP Response] Status: {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new APIResponse<PensionDetail>
                    {
                        Message = "Admin not logged in."
                    };
                }
                else
                {
                    string errorString = await httpResponse.Content.ReadAsStringAsync();
                    return new APIResponse<PensionDetail>
                    {
                        Message = string.IsNullOrWhiteSpace(errorString) ? "Something went wrong." : errorString
                    };
                }
            }

            string responseString = await httpResponse.Content.ReadAsStringAsync();


            PensionDetail pensionDetail;
            try
            {
                pensionDetail = JsonSerializer.Deserialize<PensionDetail>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<PensionDetail>
                {
                    Message = "Unable to get pension details."
                };
            }

            return new APIResponse<PensionDetail>
            {
                Success = true,
                Response = pensionDetail,
                Message = "Pension details fetched successfully!"
            };
        }

        public async Task<APIResponse<ProcessPensionInfo>> ProcessPension(ProcessPensionInput processPensionInput)
        {
            _processPensionClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Token}");

            StringContent requestContent = new StringContent(JsonSerializer.Serialize(processPensionInput), Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                string url = "/api/processPension/processPension";

                _logger.LogInformation($"[HTTP Request] POST : {_processPensionClient.BaseAddress + url}");

                httpResponse = await _processPensionClient.PostAsync("api/processPension/processPension", requestContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<ProcessPensionInfo>
                {
                    Message = ex.Message
                };
            }

            _logger.LogInformation($"[HTTP Response] Status: {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new APIResponse<ProcessPensionInfo>
                    {
                        Message = "Admin not logged in."
                    };
                }
                else
                {
                    return new APIResponse<ProcessPensionInfo>
                    {
                        Message = "Something went wrong. Try again later."
                    };
                }
            }

            string responseString = await httpResponse.Content.ReadAsStringAsync();

            ProcessPensionInfo processPensionInfo;
            try
            {
                processPensionInfo = JsonSerializer.Deserialize<ProcessPensionInfo>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<ProcessPensionInfo>
                {
                    Message = "Process pension failed."
                };
            }

            return new APIResponse<ProcessPensionInfo>
            {
                Success = true,
                Response = processPensionInfo,
                Message = "Pension processed successfully!"
            };
        }
    }
}
