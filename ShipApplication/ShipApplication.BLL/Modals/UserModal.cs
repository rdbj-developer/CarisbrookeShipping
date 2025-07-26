using Newtonsoft.Json;
using System;

namespace ShipApplication.BLL.Modals
{
    public class UserModal
    {
        public int UID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        public string Rank { get; set; }

    }
    public class ShipUserReq
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
    public class UserModalAWS
    {
        public int UID { get; set; }
        public string empnre01 { get; set; }
        public string empste01 { get; set; }
        public string fstnme01 { get; set; }
        public string surnme01 { get; set; }
        public string bthcnte01 { get; set; }
        public string fnce01 { get; set; }
        public string lane01 { get; set; }
        public string rsnewe01 { get; set; }
        public string sxee01 { get; set; }
        public Nullable<System.DateTime> bthdate01 { get; set; }
        public string nate01 { get; set; }
    }

    //RDBJ 09/16/2021
    public class CSShipsModalAWS
    {
        public int ShipId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int ShipClassId { get; set; }
        public int BuildCountryId { get; set; }
        public int BuildYear { get; set; }
        public int ClassificationSocietyId { get; set; }
        public int FlagStateId { get; set; }
        public int IMONumber { get; set; }
        public string CallSign { get; set; }
        public int MMSI { get; set; }
        public int GrossTonnage { get; set; }
        public int NetTonnage { get; set; }
        public int OfficeId { get; set; }
        public int TechnicalManagerId { get; set; }
        public int SuperintendentId { get; set; }
        public string Notes { get; set; }
        public bool? IsDelivered { get; set; }
        public int FleetId { get; set; }
        public int YardNo { get; set; }
        public int OfficialNumber { get; set; }
        public int PortOfRegistryId { get; set; }
        public decimal? SummerDeadweight { get; set; }
        public decimal? Lightweight { get; set; }
        public decimal? LOA { get; set; }
        public decimal? LBP { get; set; }
        public decimal? Beam { get; set; }
        public decimal? SummerDraft { get; set; }
        public int BHP { get; set; }
        public int BowThruster { get; set; }
        public int BuildNumber { get; set; }
        public string Agent { get; set; }
        public string Ports { get; set; }
        public string TechnicalManagerNotes { get; set; }
        public int MinimumSafeManning { get; set; }
        public int MaximumPersonsLSA { get; set; }
        public int TotalBerths { get; set; }
        public string RegisteredOwners { get; set; }
        public int HullAndMachineryId { get; set; }
        public int ProtectionAndIndemnityId { get; set; }
        public string Owner { get; set; }
        public bool? SMCIsVerified { get; set; }
        public string MarineSoftwareNumber { get; set; }
        public string Email { get; set; }
        public string DataloyNumber { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Mobile { get; set; }
        public string SatC1 { get; set; }
        public string SatC2 { get; set; }
        public string Citadel { get; set; }
        [JsonProperty(PropertyName = "P&I Club")]
        public string PandIClub { get; set; }
    }
    //End RDBJ 09/16/2021
}
