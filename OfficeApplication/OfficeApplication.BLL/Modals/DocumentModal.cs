using System;

namespace OfficeApplication.BLL.Modals
{
    public class DocumentModal
    {
        public int DocID { get; set; }
        public Nullable<System.Guid> DocumentID { get; set; }
        public Nullable<System.Guid> ParentID { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string DownloadPath { get; set; }
        public string UploadType { get; set; }
        public double? Version { get; set; }
        public double? DocumentVersion { get; set; }
        public string Location { get; set; }
        public int? DocNo { get; set; }
        public string SectionType { get; set; }
        public Nullable<bool> IsUpdateRequired { get; set; }
        public long? RAFID { get; set; }
        public bool? IsWebPage { get; set; }
        public long? RAFId1 { get; set; }
    }

    public class DocumentFolders
    {
        public string DocumentID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public double Version { get; set; }
    }
}
