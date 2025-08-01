//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarisbrookeShippingAPI.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Form
    {
        public int ID { get; set; }
        public Nullable<System.Guid> FormID { get; set; }
        public string TemplatePath { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public Nullable<int> Issue { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<int> Amendment { get; set; }
        public Nullable<System.DateTime> AmendmentDate { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string AccessLevel { get; set; }
        public Nullable<bool> AllowsNetworkAccess { get; set; }
        public Nullable<bool> CanBeOpened { get; set; }
        public Nullable<bool> HasSavedData { get; set; }
        public Nullable<bool> IsURNBased { get; set; }
        public string URN { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string DownloadPath { get; set; }
        public string UploadType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<double> DocumentVersion { get; set; }
        public Nullable<double> Version { get; set; }
        public string FolderType { get; set; }
    }
}
