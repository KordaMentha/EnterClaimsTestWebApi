using Microsoft.Identity.Client;

namespace EnterClaimsTestWebApi
{
    public class DataService
    {
        private readonly IConfiguration _configuration;

        public DataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetAccessToken()
        {
            // Get values from azure environment variables
            string clientId = Environment.GetEnvironmentVariable("SamTestClaims_clientID").ToString();
            string tenantId = Environment.GetEnvironmentVariable("SamTestClaims_tenantID").ToString();
            string clientSecret = Environment.GetEnvironmentVariable("SamTestClaims_clientsecret").ToString();

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();
            
            string[] scopes = new[] { _configuration["Dataverse:EnvironmentUrl"] + "/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        public string GetDataverseEnvironmentUrl()
        {
            return _configuration["Dataverse:EnvironmentUrl"];
        }

        public string ConvertDateToISO8601(DateTime dateTime)
        {
            // Ensure the DateTime is in UTC
            var utcDateTime = dateTime.ToUniversalTime();

            // Format as ISO 8601 with 'Z' to indicate UTC
            return utcDateTime.ToString("yyyy-MM-dd");
        }

    }
}
