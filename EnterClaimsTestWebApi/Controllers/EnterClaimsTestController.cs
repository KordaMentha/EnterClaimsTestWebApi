using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
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
        private readonly ILogger<EnterClaimsTestController> _logger;
        private readonly DataService _service;

        public EnterClaimsTestController(DataService dataService, ILogger<EnterClaimsTestController> logger)
        {
            _service = dataService;
            _logger = logger;
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
                var coreDataObject = System.Text.Json.JsonSerializer.Deserialize<DataverseResponse>(jsonResponse, new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true }
                );
                return Ok(coreDataObject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCoreDataForUserAllFields");
                return StatusCode(500, $"Unexpected error - GetCoreDataForUserAllFields: {ex.Message}"); // fallback
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
                    return StatusCode(500, $"Dataverse API call failed: {errorContent}");
                }

                string jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonResponse);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("value", out JsonElement valueArray) &&
                    valueArray.ValueKind == JsonValueKind.Array &&
                    valueArray.GetArrayLength() > 0)
                {
                    var firstItem = valueArray[0];

                    if (firstItem.TryGetProperty("km_claimformid", out JsonElement idElement))
                    {
                        return Ok(idElement.GetString() ?? "km_claimformid_is_null");
                    }
                    else
                    {
                        _logger.LogError("GetClaimIDForUser - Field 'km_claimformid' not found in the first item of the response.");
                        return StatusCode(500, "GetClaimIDForUser - Field 'km_claimformid' not found in the first item.");
                    }
                }
                else
                {
                    _logger.LogError("GetClaimIDForUser - No items found in 'value' array of the response.");
                    return StatusCode(500, "No items found in 'value' array.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error - GetClaimIDForUser: {ex.Message}");
            }
        }

        [HttpGet("UpdateClaimForm")]
        public async Task<IActionResult> UpdateClaimForm(string empID, string DOB, string last4digitsofTFN)
        {
            try
            {
                var result = await GetClaimIDForUser(empID, DOB, last4digitsofTFN);

                if (result is OkObjectResult okResult && okResult.Value is string claimIDString)
                {
                    if (Guid.TryParse(claimIDString, out Guid claimID))
                    {
                        var coreDataResult = await GetCoreDataForUserAllFields(empID, DOB, last4digitsofTFN);

                        if (coreDataResult is OkObjectResult coreOk && coreOk.Value is DataverseResponse coreDataResponse && coreDataResponse.Value.Count != 0)
                        {
                            var coreData = coreDataResponse.Value.First();

                            var updateResult = await UpdateCoreDataInClaimFormTable(coreData, claimID);

                            if (updateResult is OkObjectResult updateOk)
                            {
                                return Ok($"Success: {updateOk.Value}");
                            }
                            else if (updateResult is ObjectResult updateError)
                            {
                                _logger.LogError("UpdateClaimForm - Error updating claim form: {Error}", updateError.Value);
                                return StatusCode(updateError.StatusCode ?? 500, $"Error: {updateError.Value}");
                            }
                            else
                            {
                                _logger.LogError("UpdateClaimForm - Unexpected result type from update.");
                                return StatusCode(500, "Unexpected result type from update.");
                            }
                        }
                        else
                        {
                            _logger.LogError("UpdateClaimForm - Core data not found or empty.");
                            return StatusCode(500, "Core data not found or empty.");
                        }
                    }
                    else
                    {
                        _logger.LogError("UpdateClaimForm - Invalid claim ID format: {ClaimIDString}", claimIDString);
                        return StatusCode(500, "Invalid claim ID format.");
                    }
                }
                else
                {
                    _logger.LogError("UpdateClaimForm - Claim ID not found or response invalid.");
                    return StatusCode(500, "Claim ID not found or response invalid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateClaimForm");
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
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
                    _logger.LogError("UpdateCoreDataInClaimFormTable - Failed to update record: {Error}", error);
                    return StatusCode((int)response.StatusCode, $"Failed to update record: {error}");
                }
                return Ok("Record updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCoreDataInClaimFormTable");
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
                    _logger.LogError("GetCoreDataVerifyUser - Dataverse API call failed: {ErrorContent}", errorContent);
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCoreDataVerifyUser");
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
                    _logger.LogError("CheckForDuplicateClaim - Dataverse API call failed: {ErrorContent}", errorContent);
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckForDuplicateClaim");
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
                    _logger.LogError("DeleteDuplicateClaim - Dataverse API call failed: {ErrorContent}", errorContent);
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteDuplicateClaim");
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
                    _logger.LogError("GetBSBs - Dataverse API call failed: {ErrorContent}", errorContent);
                    return StatusCode((int)clientResponse.StatusCode, $"Dataverse API call failed: {errorContent}");
                }
                var jsonResponse = await clientResponse.Content.ReadAsStringAsync();
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBSBs");
                return StatusCode(500, $"Unexpected error: {ex.Message}"); // fallback
            }
        }
    }
}
