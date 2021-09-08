using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PensionManagementPortal.Models;
using PensionManagementPortal.Services;

namespace PensionManagementPortal.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _authClient;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AuthService authservice, ILogger<AuthRepository> logger)
        {
            _authClient = authservice.AuthClient;
            _logger = logger;
        }

        public async Task<APIResponse<AuthToken>> Login(UserCredential userCredential)
        {
            StringContent requestContent = new StringContent(JsonSerializer.Serialize(userCredential), Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                string url = "api/auth/login";

                _logger.LogInformation($"[HTTP Request] POST : {_authClient.BaseAddress + url}");

                httpResponse = await _authClient.PostAsync(url, requestContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<AuthToken>
                {
                    Message = "Something went wrong. Try again later."
                };
            }

            _logger.LogInformation($"[HTTP Response] Status: {httpResponse.StatusCode}");

            if(!httpResponse.IsSuccessStatusCode)
            {
                string errorResponseString = await httpResponse.Content.ReadAsStringAsync();
                string errorString = JsonSerializer.Deserialize<string>(errorResponseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return new APIResponse<AuthToken>
                {
                    Message = string.IsNullOrWhiteSpace(errorString) ? "Unable to login at the moment. Try again later." : errorString
                };
            }

            string responseString = await httpResponse.Content.ReadAsStringAsync();

            AuthToken authToken;
            try
            {
                authToken = JsonSerializer.Deserialize<AuthToken>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                _logger.LogError($"{ex.StackTrace}");

                return new APIResponse<AuthToken>
                {
                    Message = "Unable to login at the moment. Try again later."
                };
            }

            return new APIResponse<AuthToken>
            {
                Success = true,
                Response = authToken,
                Message = "Login successfull!"
            };
        }
    }
}
