using System;
using System.Collections.Generic;

namespace CarisbrookeShippingService.BLL.Modals
{
    public class SMRModal
    {
        public long SMRFormID { get; set; }
        public int? ShipID { get; set; }
        public string ShipName { get; set; }
        public int? Year { get; set; }
        public string ReviewPeriod { get; set; }
        public string DateOfMeeting { get; set; }
        public string Section1 { get; set; }
        public string Section2 { get; set; }
        public string Section3 { get; set; }
        public string Section4 { get; set; }
        public string Section5 { get; set; }
        public string Section6 { get; set; }
        public string Section7 { get; set; }
        public string Section7a { get; set; }
        public string Section7b { get; set; }
        public string Section7c { get; set; }
        public string Section7d { get; set; }
        public string Section7e1 { get; set; }
        public string Section7e2 { get; set; }
        public string Section7e3 { get; set; }
        public string Section7f1 { get; set; }
        public string Section7f2 { get; set; }
        public string Section7g { get; set; }
        public string Section7h { get; set; }
        public string Section8a { get; set; }
        public string Section8b { get; set; }
        public string Section8b1 { get; set; }
        public string Section8b2 { get; set; }
        public string Section8b3 { get; set; }
        public string Section8b4 { get; set; }
        public string Section8b5 { get; set; }
        public string Section9 { get; set; }
        public string Section10 { get; set; }
        public string Section11 { get; set; }
        public string Section12a { get; set; }
        public string Section12b { get; set; }
        public string Section12c { get; set; }
        public string Section12d { get; set; }
        public string Section12e { get; set; }
        public string Section12f { get; set; }
        public string Section12g { get; set; }
        public string Section12h { get; set; }
        public string Section12i { get; set; }
        public string Section12j { get; set; }
        public string Section12k { get; set; }
        public string Section13 { get; set; }
        public bool IsSynced { get; set; }
        public List<SMRFormCrewMemberModal> SMRFormCrewMemberList { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class SMRFormCrewMemberModal
    {
        public long CrewMemberID { get; set; }
        public long SMRFormID { get; set; }
        public string Rank { get; set; }
        public string FullName { get; set; }
        public bool ElectedAsSafety { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class OfflineSMRFormModal
    {
        public int SMRFormID { get; set; }
        public bool Synced { get; set; }
        public SMRModal SMRForm { get; set; }
    }
}
