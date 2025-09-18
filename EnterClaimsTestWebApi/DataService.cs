using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace EnterClaimsTestWebApi
{
    public class DataService
    {
        public DataService()
        {
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

        public async Task<string> GetBSBAsync()
        {
            string clientId = "6cb3ba02-5b82-4146-8654-23fa655e641e";
            string tenantId = "b6506b08-d2b9-4446-9f9b-f639002bc340";
            string clientSecret = "8GC8Q~3jjqNCNOw.78DTX7qOb4G8C1UIsEnIlcRt";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/km_bsbs";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(apiUrl);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
