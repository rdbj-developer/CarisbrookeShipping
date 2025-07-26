using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class CSShipsModal
    {
        public int ShipId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Nullable<int> ShipClassId { get; set; }
        public Nullable<int> BuildCountryId { get; set; }
        public string ClassSociety { get; set; }
        public Nullable<int> BuildYear { get; set; }
        public Nullable<int> ClassificationSocietyId { get; set; }
        public Nullable<int> FlagStateId { get; set; }
        public Nullable<int> IMONumber { get; set; }
        public string CallSign { get; set; }
        public Nullable<int> MMSI { get; set; }
        public Nullable<int> GrossTonnage { get; set; }
        public Nullable<int> NetTonnage { get; set; }
        public Nullable<int> OfficeId { get; set; }
        public Nullable<int> TechnicalManagerId { get; set; }
        public Nullable<int> SuperintendentId { get; set; }
        public string Notes { get; set; }
        public bool IsDelivered { get; set; }
        public Nullable<int> FleetId { get; set; }
        public Nullable<int> YardNo { get; set; }
        public Nullable<int> OfficialNumber { get; set; }
        public Nullable<int> PortOfRegistryId { get; set; }
        public Nullable<decimal> SummerDeadweight { get; set; }
        public Nullable<decimal> Lightweight { get; set; }
        public Nullable<decimal> LOA { get; set; }
        public Nullable<decimal> LBP { get; set; }
        public Nullable<decimal> Beam { get; set; }
        public Nullable<decimal> SummerDraft { get; set; }
        public Nullable<int> BHP { get; set; }
        public Nullable<int> BowThruster { get; set; }
        public Nullable<int> BuildNumber { get; set; }
        public string Agent { get; set; }
        public string Ports { get; set; }
        public string TechnicalManagerNotes { get; set; }
        public Nullable<int> MinimumSafeManning { get; set; }
        public Nullable<int> MaximumPersonsLSA { get; set; }
        public Nullable<int> TotalBerths { get; set; }
        public string RegisteredOwners { get; set; }
        public Nullable<int> HullAndMachineryId { get; set; }
        public Nullable<int> ProtectionAndIndemnityId { get; set; }
        public string Owner { get; set; }
        public bool SMCIsVerified { get; set; }
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
        public string P_I_Club { get; set; }
    }
}
