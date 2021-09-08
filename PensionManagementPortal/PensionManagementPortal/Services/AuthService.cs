using System;
using System.Net.Http;

namespace PensionManagementPortal.Services
{
    public class AuthService
    {
        public HttpClient AuthClient { get; private set; }
        public AuthService(HttpClient httpClient)
        {
            AuthClient = httpClient;
        }
    }
}
