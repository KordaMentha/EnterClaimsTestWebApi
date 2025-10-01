using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        [HttpGet("GetCoreDataForUserAllFields")]
        public async Task<IActionResult> GetCoreDataForUserAllFields(string empID, string DOB, string last4digitsofTFN) { //get all core data fields
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_coredatas?$filter=km_employeeid eq '" + empID + "'" +
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
                //return Content(jsonResponse, "application/json");
                var coreDataObject = System.Text.Json.JsonSerializer.Deserialize<DataverseResponse>(jsonResponse, new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true }
                );
                return Ok(coreDataObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }

        [HttpGet("GetClaimIDForUser")]
        public async Task<IActionResult> GetClaimIDForUser(string empID, string DOB, string last4digitsofTFN)
        {
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_claimforms?$select=km_claimformid" +
                    "&$filter=km_employeeid eq '" + empID + "'" +
                    " and km_dateofbirth eq '" + DOB + "'" +
                    " and endswith(km_validatelastfourdigitstfn, '" + last4digitsofTFN + "')";

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
                return Ok(Content(jsonResponse, "application/json"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }


        [HttpGet("UpdateClaimForm")]
        public async Task<string> UpdateClaimForm(string empID, string DOB, string last4digitsofTFN)
        {
            Guid claimID = Guid.Parse("fa6b5880-9f9d-f011-aa43-002248e21c76");
            //string claimID = "fa6b5880-9f9d-f011-aa43-002248e21c76"; //test claim record ID to update
            var result = await GetCoreDataForUserAllFields(empID, DOB, last4digitsofTFN) as OkObjectResult;
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (result?.Value is DataverseResponse response && response.Value.Count != 0)
            {
                var coreData = response.Value.First();

                var updateResult = await UpdateCoreDataInClaimFormTable(coreData, claimID);

                if (updateResult is OkObjectResult okResult)
                {
                    return($"Success: {okResult.Value}");
                }
                else if (updateResult is ObjectResult errorResult)
                {
                    return($"Error: {errorResult.StatusCode} - {errorResult.Value}");
                }
                else
                {
                    return("Unexpected result type.");
                }
            }
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            return "User not found or data missing.";
        }

        private async Task<IActionResult> UpdateCoreDataInClaimFormTable(CoreDataModel coreData, Guid recordId)
        {
            try
            {
                var payLoad = new ClaimsDataModel
                {
                    km_firstname = coreData.EmployeeFirstName,
                    km_surname = coreData.EmployeeSurName,
                    km_taxfilenumbertfn = coreData.TFN,
                    km_addressline1 = coreData.AddressLine1,
                    km_addressline2 = coreData.AddressLine2,
                    km_suburb = coreData.Suburb,
                    km_postcode = coreData.PostCode,
                    km_currentorpastemployment = coreData.CurrentOrPastEmploymentClaim,
                    km_positiontitle = coreData.PositionTitle,
                    km_startdate = coreData.StartDate,
                    km_terminationdate = coreData.TerminationDate,
                    km_currentpayrate = coreData.CurrentPay,
                    km_lastpayrate = coreData.LastPayRate,
                    km_annualleavehourstobecredited = coreData.AnnualLeaveHoursToBeCredited,
                    km_personalleavehourstobecredited = coreData.PersonalLeaveHoursToBeCredited,
                    km_leaveloading = coreData.LeaveLoading,
                    km_entitlementvaluebeforeinterest = coreData.EntitlementValueBeforeInterest,
                    km_interestcalculatedonleaveentitlement = coreData.InterestCalculatedOnLeaveEntitlement,
                    km_grossentitlement = coreData.GrossEntitlement,
                    km_taxwithheld = coreData.TaxWithHeld,
                    km_netpayment = coreData.NetPayment,
                    km_workertype = coreData.WorkerType,
                    //km_paypoint = coreData.PayPoint, //daatype mismatch int vs string in claim form table
                    km_standardweeklyhours = coreData.StandardWeeklyHours,
                    km_lasttermdate = coreData.LastTermDate?.ToString("yyyy-MM-dd"),
                    km_servicestartdate = coreData.ServiceStartDate,
                    km_bankaccountname = coreData.BankAccountName,
                    km_bankaccountbsb = coreData.BSB,
                    km_bankaccountnumber = coreData.BankAccountNumber
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(payLoad, new JsonSerializerOptions { PropertyNamingPolicy = null, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull});
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string apiUrl = _service.GetDataverseEnvironmentUrl() + $"/api/data/v9.2/km_claimforms({recordId})";
                string token = await _service.GetAccessToken();

                var request = new HttpRequestMessage(HttpMethod.Patch, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = content;

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Failed to update record: {error}");
                }

                return Ok("Record updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

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
                return Ok(Content(jsonResponse, "application/json"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }

        [HttpGet("CheckForDuplicateClaim")]
        public async Task<IActionResult> CheckForDuplicateClaim(string empID, string DOB, string last4digitsofTFN)
        {
            try
            {
                string apiUrl = _service.GetDataverseEnvironmentUrl() + "/api/data/v9.2/km_claimforms?$select=km_claimformid" +
                    "&$filter=km_employeeid eq '" + empID + "'" +
                    " and km_dateofbirth eq '" + DOB + "'" +
                    " and endswith(km_validatelastfourdigitstfn, '" + last4digitsofTFN + "')";

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
                return Ok(Content(jsonResponse, "application/json"));
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
                return Ok(Content(jsonResponse, "application/json"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }

        //[Authorize(Policy = "Api.Read")]
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
                return Ok(Content(jsonResponse, "application/json"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }
    }
}
