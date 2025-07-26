using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.Modals
{
    public class ShipAppReleaseNoteModal
    {
        public long AppId { get; set; }
        public string AppVersion { get; set; }
        public string AppDescription { get; set; }
        public DateTime? AppPublishDate { get; set; }
        public int? NoOfFilesAffected { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
    public class ShipAppDownloadLogModal
    {
        public Guid Id { get; set; }
        public string ShipCode { get; set; }
        public string PCName { get; set; }
        public string PCUniqueId { get; set; }
        public long? OldAppId { get; set; }
        public long? DownloadedAppId { get; set; }
        public DateTime? DownloadDate { get; set; }
    }
    public class ShipSystemModal
    {
        public long Id { get; set; }
        public string ShipCode { get; set; }
        public string PCName { get; set; }
        public string PCUniqueId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
    }

    public class MainSyncServicesEventLogModal
    {
        public Guid Id { get; set; }
        public long? ShipSystemId { get; set; }
        public string RunningVersion { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
