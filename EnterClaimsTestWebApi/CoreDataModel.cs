using System.Text.Json.Serialization;

namespace EnterClaimsTestWebApi
{
    public class CoreDataModel
    {
        [JsonPropertyName("km_coredataid")]
        public string? CoreDataID { get; set; }
        [JsonPropertyName("km_employeefirstname")]
        public string? EmployeeFirstName { get; set; }
        [JsonPropertyName("km_employeesurname")]
        public string? EmployeeSurName { get; set; }
        [JsonPropertyName("km_dateofbirth")]
        public DateTimeOffset? DateOfBirth { get; set; }
        [JsonPropertyName("km_homephone")]
        public string? HomePhone { get; set; }
        [JsonPropertyName("km_mobilephone")]
        public string? MobileNumber { get; set; }
        [JsonPropertyName("km_tfn")]
        public string? TFN { get; set; }
        [JsonPropertyName("km_email")]
        public string? Email { get; set; }
        [JsonPropertyName("km_addressline1")]
        public string? AddressLine1 { get; set; }
        [JsonPropertyName("km_addressline2")]
        public string? AddressLine2 { get; set; }
        [JsonPropertyName("km_suburb")]
        public string? Suburb { get; set; }
        [JsonPropertyName("km_postcode")]
        public string? PostCode { get; set; }
        [JsonPropertyName("km_state")]
        public string? State { get; set; }
        [JsonPropertyName("km_employeeid")]
        public string? EmployeeID { get; set; }
        [JsonPropertyName("km_currentorpastemploymentclaim")]
        public int? CurrentOrPastEmploymentClaim { get; set; }
        [JsonPropertyName("km_positiontitle")]
        public string? PositionTitle { get; set; }
        [JsonPropertyName("km_startdate")]
        public DateTimeOffset? StartDate { get; set; }
        [JsonPropertyName("km_terminationdate")]
        public DateTimeOffset? TerminationDate { get; set; }
        [JsonPropertyName("km_currentpayrate")]
        public decimal? CurrentPay { get; set; }
        [JsonPropertyName("km_lastpayrate")]
        public decimal? LastPayRate { get; set; }
        [JsonPropertyName("km_annualleavehourstobecredited")]
        public decimal? AnnualLeaveHoursToBeCredited { get; set; }
        [JsonPropertyName("km_personalleavehourstobecredited")]
        public decimal? PersonalLeaveHoursToBeCredited { get; set; }
        [JsonPropertyName("km_leaveloading")]
        public decimal? LeaveLoading { get; set; }
        [JsonPropertyName("km_entitlementvaluebeforeinterest")]
        public decimal? EntitlementValueBeforeInterest { get; set; }
        [JsonPropertyName("km_interestcalculatedonleaveentitlement")]
        public decimal? InterestCalculatedOnLeaveEntitlement { get; set; }
        [JsonPropertyName("km_grossentitlement")]
        public decimal? GrossEntitlement { get; set; }
        [JsonPropertyName("km_taxwithheld")]
        public decimal? TaxWithHeld { get; set; }
        [JsonPropertyName("km_netpayment")]
        public decimal? NetPayment { get; set; }
        [JsonPropertyName("km_workertype")]
        public string? WorkerType { get; set; }
        [JsonPropertyName("km_awardid")]
        public string? AwardID { get; set; }
        [JsonPropertyName("km_paypoint")]
        public int? PayPoint { get; set; }
        [JsonPropertyName("km_standardweeklyhours")]
        public decimal? StandardWeeklyHours { get; set; }
        [JsonPropertyName("km_lasttermdate")]
        public DateTimeOffset? LastTermDate { get; set; }
        [JsonPropertyName("km_latehiredate")]
        public DateTimeOffset? LateHireDate { get; set; }
        [JsonPropertyName("km_servicestartdate")]
        public DateTimeOffset? ServiceStartDate { get; set; }
        [JsonPropertyName("km_bankaccountname")]
        public string? BankAccountName { get; set; }
        [JsonPropertyName("km_bsb")]
        public string? BSB { get; set; }
        [JsonPropertyName("km_bankaccountnumber")]
        public string? BankAccountNumber { get; set; }
    }

    public class DataverseResponse
    {
        [JsonPropertyName("value")]
        public List<CoreDataModel>? Value { get; set; }
    }

}
