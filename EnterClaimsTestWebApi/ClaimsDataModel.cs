using System.Text.Json.Serialization;

namespace EnterClaimsTestWebApi
{
    public class ClaimsDataModel
    {
        public string? km_firstname { get; set; }
        public string? km_surname { get; set; }
        public DateTimeOffset? km_dateofbirth { get; set; }
        public string? km_homephone { get; set; }
        public string? km_phonemobilenumber { get; set; }
        public string? km_taxfilenumbertfn { get; set; }
        public string? km_emailaddress { get; set; }
        public string? km_addressline1 { get; set; }
        public string? km_addressline2 { get; set; }
        public string? km_suburb { get; set; }
        public string? km_postcode { get; set; }
        public string? km_employeeid { get; set; }
        public int? km_currentorpastemployment { get; set; }
        public string? km_positiontitle { get; set; }
        public DateTimeOffset? km_startdate { get; set; }
        public DateTimeOffset? km_terminationdate { get; set; }
        public decimal? km_currentpayrate { get; set; }
        public decimal? km_lastpayrate { get; set; }
        public decimal? km_annualleavehourstobecredited { get; set; }
        public decimal? km_personalleavehourstobecredited { get; set; }
        public decimal? km_leaveloading { get; set; }
        public decimal? km_entitlementvaluebeforeinterest { get; set; }
        public decimal? km_interestcalculatedonleaveentitlement { get; set; }
        public decimal? km_grossentitlement { get; set; }
        public decimal? km_taxwithheld { get; set; }
        public decimal? km_netpayment { get; set; }
        public string? km_workertype { get; set; }
        public int? km_paypoint { get; set; }
        public decimal? km_standardweeklyhours { get; set; }
        public string? km_lasttermdate { get; set; }
        public DateTimeOffset? km_servicestartdate { get; set; }
        public string? km_bankaccountname { get; set; }
        public string? km_bankaccountbsb { get; set; }
        public string? km_bankaccountnumber { get; set; }
    }
}
