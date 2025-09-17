using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace EnterClaimsTestWebApi
{
    public class DataverseService
    {
        private readonly IConfiguration _config;

        public DataverseService(IConfiguration config)
        {
            _config = config;
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
            string clientId = "e6b9d490-4181-4419-ad40-21df0ad14077";
            string tenantId = "b6506b08-d2b9-4446-9f9b-f639002bc340";
            string clientSecret = "7fdf70d4-285e-4530-aa31-b833c250accf";
            string apiUrl = "https://tectestimportsolution-dev.crm6.dynamics.com/api/data/v9.2/km_bsbs";

            string token = await GetAccessTokenAsync(clientId, tenantId, clientSecret);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.GetAsync(apiUrl);
            return await response.Content.ReadAsStringAsync();
        }


        //public async Task<string> GetBSBAsyncOLD()
        //{
        //    var clientId = _config["Dataverse:ClientId"];
        //    var tenantId = _config["Dataverse:TenantId"];
        //    var clientSecret = _config["Dataverse:ClientSecret"];
        //    var dataverseUrl = _config["Dataverse:DataverseUrl"];

        //    var scopes = new[] { $"{dataverseUrl}/.default" };
        //    var options = new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };
        //    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

        //    var token = await credential.GetTokenAsync(new TokenRequestContext(scopes));
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        //    var response = await httpClient.GetAsync($"{dataverseUrl}/api/data/v9.2/km_bsbs");
        //    return await response.Content.ReadAsStringAsync();
        //}

    }
}
