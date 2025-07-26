using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class GIRData
    {
        public Nullable<System.Guid> UniqueFormID { get; set; } // RDBJ 12/03/2021
        public long GIRFormID { get; set; }
        public string Date { get; set; }
        public long? NoDefects { get; set; }
        public long? Outstending { get; set; }
        public string Location { get; set; }
        public string Auditor { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string ReportType { get; set; }
        public int GIDeficiencies { get; set; }
        public int OpenGIDeficiencies { get; set; }
        public int SIDeficiencies { get; set; }
        public int OpenSIDeficiencies { get; set; }
        public string GeneralPreamble { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
    public class GIRDataList
    {
        public GIRDataList()
        {
            GIRDeficienciesFile = new List<GIRDeficienciesFile>();
        }
        public long? GIRFormID { get; set; }
        public Nullable<System.Guid> DeficienciesUniqueID { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public string Deficiency { get; set; }
        public long DeficienciesID { get; set; }
        public bool? IsColse { get; set; }
        public string Number { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string ReportType { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool isExpired { get; set; }
        public string Section { get; set; }
        public string Inspector { get; set; }
        public string Port { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // RDBJ 12/14/2021 
        public List<GIRDeficienciesFile> GIRDeficienciesFile { get; set; }
        public string AssignTo { get; set; } // RDBJ 12/21/2021
    }
}
