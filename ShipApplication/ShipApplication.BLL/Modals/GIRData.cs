using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class GIRData
    {
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public long GIRFormID { get; set; }
        public string Date { get; set; }
        public long? NoDefects { get; set; }
        public long? Outstending { get; set; }
        public string Location { get; set; }
        public string Auditor { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string ReportType { get; set; }
        public string GeneralPreamble { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
    public class GIRDataList
    {
        public GIRDataList()
        {
            GIRDeficienciesFile = new List<GIRDeficienciesFile>();
            GIRDeficienciesInitialActionsFiles = new List<GIRDeficienciesInitialActionsFile>();
            GIRDeficienciesResolutionFiles = new List<GIRDeficienciesResolutionFile>();
            GIRDeficienciesCommentFile = new List<GIRDeficienciesCommentFile>();
        }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public Nullable<System.Guid> DeficienciesUniqueID { get; set; }
        public long? GIRFormID { get; set; }
        public string Deficiency { get; set; }
        public long DeficienciesID { get; set; }
        public bool? IsClose { get; set; } //RDBJ 10/26/2021 Rename from IsColse to IsClose
        public string Number { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string ReportType { get; set; }
        public int No { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool isExpired { get; set; }
        public string Section { get; set; }
        public string Inspector { get; set; }
        public string Port { get; set; }
        public int Priority { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }    // JSL 06/13/2022
        public DateTime? CreatedDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }   // RDBJ 03/01/2022
        public List<GIRDeficienciesFile> GIRDeficienciesFile { get; set; }
        public List<GIRDeficienciesInitialActionsFile> GIRDeficienciesInitialActionsFiles { get; set; }
        public List<GIRDeficienciesResolutionFile> GIRDeficienciesResolutionFiles { get; set; }
        public List<GIRDeficienciesCommentFile> GIRDeficienciesCommentFile { get; set; }

    }
}
