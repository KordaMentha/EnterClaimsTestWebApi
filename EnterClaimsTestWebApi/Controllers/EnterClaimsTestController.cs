using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;

namespace EnterClaimsTestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterClaimsTestController : ControllerBase
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly DataService _service;

        public EnterClaimsTestController(DataService dataService)
        {
            _service = dataService;
        }

        [HttpGet("GetCoreDataForUser")]
        public async Task<IActionResult> GetCoreDataForUser(string empID, string DOB, string last4digitsofTFN)
        {
            string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_coredatas?$filter=km_employeeid eq '" + empID + "'" +
                " and km_dateofbirth eq '" + DOB + "'" + " and endswith(km_tfn, '" + last4digitsofTFN + "')";

            string token = await _service.GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());
        }

        [HttpGet("GetCoreData")]
        public async Task<IActionResult> GetCoreData()
        {
            string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_coredatas";
            string token = await _service.GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());
        }

        [HttpGet("GetBSBs")]
        public async Task<IActionResult> GetBSBs()
        {
            string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_bsbs";
            string token = await _service.GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

        }

        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts()
        {
            string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/accounts";
            string token = await _service.GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

        }

        [HttpGet("GetContacts")]
        public async Task<IActionResult> GetContacts()
        {
            string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/contacts";
            string token = await _service.GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var clientResponse = httpClient.SendAsync(request).Result;
            return Ok(await clientResponse.Content.ReadAsStringAsync());

        }
    }
}
