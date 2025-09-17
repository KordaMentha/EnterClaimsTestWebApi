using Azure.Core;
using Azure.Identity;
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

        public async Task<string> GetBSBAsync()
        {
            var clientId = _config["Dataverse:ClientId"];
            var tenantId = _config["Dataverse:TenantId"];
            var clientSecret = _config["Dataverse:ClientSecret"];
            var dataverseUrl = _config["Dataverse:DataverseUrl"];

            var scopes = new[] { $"{dataverseUrl}/.default" };
            var options = new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            var token = await credential.GetTokenAsync(new TokenRequestContext(scopes));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var response = await httpClient.GetAsync($"{dataverseUrl}/api/data/v9.2/km_bsbs");
            return await response.Content.ReadAsStringAsync();
        }

    }
}
