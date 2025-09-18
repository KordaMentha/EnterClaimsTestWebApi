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
        private readonly DataService _dataverseService;
        private static readonly HttpClient httpClient = new HttpClient();

        public EnterClaimsTestController(DataService dataverseService)
        {
            _dataverseService = dataverseService;
        }

        private async Task<string> GetAccessTokenAsync(string clientId, string tenantId, string clientSecret)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            string[] scopes = new[] { "https://tectestimportsolution-dev.crm6.dynamics.com/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        [HttpGet("GetAllBSBs1")]
        public async Task<IActionResult> GetBSBAsync()
        {
            string clientId = "6cb3ba02-5b82-4146-8654-23fa655e641e";
            string tenantId = "b6506b08-d2b9-4446-9f9b-f639002bc340";
            string clientSecret = "8GC8Q~3jjqNCNOw.78DTX7qOb4G8C1UIsEnIlcRt";
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


        [HttpGet("GetAllBSBs2")]
        public async Task<IActionResult> GetAllBSBs()
        {
            var result = await _dataverseService.GetBSBAsync();
            return Ok(result);
        }

    }
}
