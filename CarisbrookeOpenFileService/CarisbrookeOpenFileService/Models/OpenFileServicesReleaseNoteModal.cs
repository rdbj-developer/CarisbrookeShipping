using System;

namespace CarisbrookeOpenFileService.Models
{
    public class OpenFileServicesReleaseNoteModal
    {
        public long AppId { get; set; }
        public string AppVersion { get; set; }
        public string AppDescription { get; set; }
        public Nullable<System.DateTime> AppPublishDate { get; set; }
        public Nullable<int> NoOfFilesAffected { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
    public class OpenFileServicesDownloadLogModal
    {
        public Guid Id { get; set; }
        public string ShipCode { get; set; }
        public string PCName { get; set; }
        public string PCUniqueId { get; set; }
        public long? OldAppId { get; set; }
        public long? DownloadedAppId { get; set; }
        public DateTime? DownloadDate { get; set; }
    }
}
