using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class GIRData
    {
        public long GIRFormID { get; set; }
        public string Date { get; set; }
        public long? NoDefects { get; set; }
        public long? Outstending { get; set; }
        public string Location { get; set; }
        public string Auditor { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string ReportType { get; set; }
    }
    public class GIRDataList
    {
        public GIRDataList()
        {
            GIRDeficienciesFile = new List<GIRDeficienciesFile>();
        }
        public long? GIRFormID { get; set; }
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
        public List<GIRDeficienciesFile> GIRDeficienciesFile { get; set; }
    }
}
