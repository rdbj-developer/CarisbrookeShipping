using System;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class OpenFileServicesEventLogModal
    {
        public Guid Id { get; set; }
        public long? ShipSystemId { get; set; }
        public string RunningVersion { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
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
    public class GeneralSettingsModal
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
    }
}
