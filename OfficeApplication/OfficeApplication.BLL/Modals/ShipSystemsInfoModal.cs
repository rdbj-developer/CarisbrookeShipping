using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class ShipSystemModal
    {
        public long Id { get; set; }
        public string ShipCode { get; set; }
        public string PCName { get; set; }
        public string PCUniqueId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
    }

    public class ShipSystemsEventLogModal
    {
        public Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public Nullable<int> EventId { get; set; }
        public string EventSource { get; set; }
        public string EventDescription { get; set; }
        public Nullable<DateTime> EventDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string UserName { get; set; }
        public string EventMachineName { get; set; }
        public string EventType { get; set; }
        public string EventLogType { get; set; }

    }
    public class ShipSystemsInfoModal
    {
        public Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string GroupName { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ShipSystemsServiceModal
    {
        public System.Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string StartupType { get; set; }
        public string StartName { get; set; }
        public string SystemName { get; set; }
        public Nullable<long> ProcessId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public String GroupName { get; set; }
    }
    public class ShipSystemsProcessModal
    {
        public System.Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public Nullable<long> ProcessId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string MemoryUsage { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }

    public class ShipWisePCModal
    {
        public long Id { get; set; }
        public string PCUniqueId { get; set; }
        public string ShipCode { get; set; }
        public string Name { get; set; }
        public string PCName { get; set; }
        public Nullable<DateTime> EventDate { get; set; }
        public Nullable<DateTime> DownloadDate { get; set; }
        public bool IsRunningOpenFileService { get; set; }
        public bool IsRunningMainSyncServices { get; set; }
        public Nullable<DateTime> OFSLastUpdateDate { get; set; }
        public Nullable<DateTime> MSSLastUpdateDate { get; set; }
        public string OFSRunningVersion { get; set; }
        public string MSSRunningVersion { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsMainPC { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string OSArchitecture { get; set; }
        public string ProductID { get; set; }
        public string UpdatePatch { get; set; }
        public string UpdateInstallDate { get; set; }
    }
    public class ShipSystemsSoftwareInfoModal
    {
        public System.Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string InstallDate { get; set; }
        public string InstallLocation { get; set; }
        public string Publisher { get; set; }
        public string UninstallString { get; set; }
        public string ModifyPath { get; set; }
        public string RepairPath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
    }
}
