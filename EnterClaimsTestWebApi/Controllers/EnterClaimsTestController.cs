using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace EnterClaimsTestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterClaimsTestController : ControllerBase
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public EnterClaimsTestController()
        {
        }

        private async Task<string> GetAccessTokenAsync(string clientId, string tenantId, string clientSecret)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            //string[] scopes = new[] { Environment.GetEnvironmentVariable("SamTestClaims_Scope").ToString() + ".default" };
            string[] scopes = new[] { "https://tectestimportsolution-dev.crm6.dynamics.com/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        [HttpGet("GetBSBs")]
        public async Task<IActionResult> GetBSBAsync()
        {
            string clientId = Environment.GetEnvironmentVariable("SamTestClaims_clientID").ToString();
            string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString();
            string clientSecret = Environment.GetEnvironmentVariable("SamTestClaims_clientsecret").ToString();
            //string apiUrl =  Environment.GetEnvironmentVariable("SamTestClaims_Scope").ToString() + "data/v9.2/km_bsbs";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/km_bsbs";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

            //using var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await httpClient.GetAsync(apiUrl);
            //return await response.Content.ReadAsStringAsync();
        }
        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts()
        {
            string clientId = Environment.GetEnvironmentVariable("SamTestClaims_clientID").ToString();
            string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString();
            string clientSecret = Environment.GetEnvironmentVariable("SamTestClaims_clientsecret").ToString();
            //string apiUrl = Environment.GetEnvironmentVariable("SamTestClaims_Scope").ToString() + "data/v9.2/contacts";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/accounts";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

            //using var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await httpClient.GetAsync(apiUrl);
            //return await response.Content.ReadAsStringAsync();
        }

        [HttpGet("GetContacts")]
        public async Task<IActionResult> GetContacts()
        {
            string clientId = Environment.GetEnvironmentVariable("SamTestClaims_clientID").ToString();
            string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString();
            string clientSecret = Environment.GetEnvironmentVariable("SamTestClaims_clientsecret").ToString();
            //string apiUrl = Environment.GetEnvironmentVariable("SamTestClaims_Scope").ToString() + "data/v9.2/contacts";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/contacts";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

            //using var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await httpClient.GetAsync(apiUrl);
            //return await response.Content.ReadAsStringAsync();
        }

        [HttpGet("GetCoreData")]
        public async Task<IActionResult> GetCoreData()
        {
            string clientId = Environment.GetEnvironmentVariable("SamTestClaims_clientID").ToString();
            string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString();
            string clientSecret = Environment.GetEnvironmentVariable("SamTestClaims_clientsecret").ToString();
            //string apiUrl = Environment.GetEnvironmentVariable("SamTestClaims_Scope").ToString() + "data/v9.2/contacts";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/km_coredatas";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

            //using var httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await httpClient.GetAsync(apiUrl);
            //return await response.Content.ReadAsStringAsync();
        }

    }
}
