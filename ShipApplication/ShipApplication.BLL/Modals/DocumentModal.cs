using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class DocumentModal
    {
        public int DocID { get; set; }
        public Nullable<System.Guid> DocumentID { get; set; }
        public Nullable<System.Guid> ParentID { get; set; }
        public string Number { get; set; }
        public int? DocNo { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string DownloadPath { get; set; }
        public bool IsDeleted { get; set; }
        public string UploadType { get; set; }
        public double? DocumentVersion { get; set; }
        public double? Version { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
       
        public string SectionType { get; set; }
        public long? RAFID { get; set; }
      //  public bool? IsWebPage { get; set; }
        public long? RAFId1 { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 12/27/2022
    }
}
