using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text.Json;

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

        //[HttpGet("GetCoreDataVerifyUser")]
        //public async Task<IActionResult> GetCoreDataVerifyUser(string empID, string DOB, string last4digitsofTFN)
        //{
        //    // Convert DOB to ISO 8601 format (yyyy-MM-ddTHH:mm:ssZ)
        //    //string isoDOB = _service.ConvertDateToISO8601(DateTime.Parse(DOB));
        //    string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_coredatas?$select=km_coreid&$filter=km_employeeid eq '" + empID + "'" +
        //        " and km_dateofbirth eq '" + DOB + "'" + " and endswith(km_tfn, '" + last4digitsofTFN + "')";
        //    string token = await _service.GetAccessToken();
        //    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var clientResponse = httpClient.SendAsync(request).Result;
        //    var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
        //    return Content(jsonResponse, "application/json");
        //}

        //[Authorize(Policy = "Api.Read")]
        [HttpGet("GetCoreDataVerifyUser")]
        public async Task<IActionResult> GetCoreDataVerifyUser(string empID, string DOB, string last4digitsofTFN)
        {
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_coredatas?$select=km_coreid" +
                    "&$filter=km_employeeid eq '" + empID + "'" +
                    " and km_dateofbirth eq '" + DOB + "'" +
                    " and endswith(km_tfn, '" + last4digitsofTFN + "')";

                string token = await _service.GetAccessToken();
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var clientResponse = await httpClient.SendAsync(request);
                if (!clientResponse.IsSuccessStatusCode)
                {
                    var errorContent = await clientResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }

        [HttpGet("DeleteDuplicateClaim")]
        public async Task<IActionResult> DeleteDuplicateClaim(string claimID)
        {
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_claimforms(" + claimID + ")";
                string token = await _service.GetAccessToken();
                var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var clientResponse = httpClient.SendAsync(request).Result;
                if (!clientResponse.IsSuccessStatusCode)
                {
                    var errorContent = await clientResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }

        [HttpGet("GetBSBs")]
        public async Task<IActionResult> GetBSBs()
        {
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_bsbs";
                string token = await _service.GetAccessToken();
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var clientResponse = httpClient.SendAsync(request).Result;
                if (!clientResponse.IsSuccessStatusCode)
                {
                    var errorContent = await clientResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }
    }
}
