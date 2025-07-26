using System;
using System.Collections.Generic;

namespace ShipApplication.BLL.Modals
{
    public class AssetManagmentEquipmentListModal
    {
        public AssetManagmentEquipmentListForm AssetManagmentEquipmentListForm { get; set; }
        public List<AssetManagmentEquipmentOTListModel> AssetManagmentEquipmentOTListModel { get; set; }
        public List<AssetManagmentEquipmentITListModel> AssetManagmentEquipmentITListModel { get; set; }
        public List<AssetManagmentEquipmentSoftwareAssetsModel> AssetManagmentEquipmentSoftwareAssetsModel { get; set; }

        public AssetManagmentEquipmentListModal()
        {
            AssetManagmentEquipmentListForm = new AssetManagmentEquipmentListForm();
            AssetManagmentEquipmentOTListModel = new List<AssetManagmentEquipmentOTListModel>();
            AssetManagmentEquipmentITListModel = new List<AssetManagmentEquipmentITListModel>();
            AssetManagmentEquipmentSoftwareAssetsModel = new List<AssetManagmentEquipmentSoftwareAssetsModel>();
        }
    }
    public class AssetManagmentEquipmentListForm
    {
        public Guid AMEId { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? SavedAsDraft { get; set; }
        public bool? IsSynced { get; set; }
    }
    public class AssetManagmentEquipmentOTListModel
    {
        public Guid OTId { get; set; }
        public Guid AMEId { get; set; }
        public string OTEquipment { get; set; }
        public string OTLocation { get; set; }
        public string OTMake { get; set; }
        public string OTModel { get; set; }
        public string OTType { get; set; }
        public string OTSerialNo { get; set; }
        public string OTWorkingCondition { get; set; }
        public string OTLastServiced { get; set; }
        public string OTRemark { get; set; }
        public string OTHardwareId { get; set; }
        public string OTOwner { get; set; }
        public string OTPersonResponsible { get; set; }
        public string OTCriticality { get; set; }
        public string OTOperatingSystem { get; set; }
        public string OTOSPatchVersion { get; set; }
    }
    public class AssetManagmentEquipmentITListModel
    {
        public Guid ITId { get; set; }
        public Guid AMEId { get; set; }
        public string ITEquipment { get; set; }
        public string ITLocation { get; set; }
        public string ITMake { get; set; }
        public string ITModel { get; set; }
        public string ITType { get; set; }
        public string ITSerialNo { get; set; }
        public string ITWorkingCondition { get; set; }
        public string ITLastServiced { get; set; }
        public string ITRemark { get; set; }
        public string ITHardwareId { get; set; }
        public string ITOwner { get; set; }
        public string ITPersonResponsible { get; set; }
        public string ITCriticality { get; set; }
        public string ITOperatingSystem { get; set; }
        public string ITOSPatchVersion { get; set; }
    }
    public class AssetManagmentEquipmentSoftwareAssetsModel
    {
        public Guid SAId { get; set; }
        public Guid AMEId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string LicenseType { get; set; }
        public string Category { get; set; }
        public string IsActive { get; set; }
        public string SASoftwareId { get; set; }
        public string SAOwner { get; set; }
        public string SAPersonResponsible { get; set; }
        public string SACriticality { get; set; }
        public string SAOperatingSystem { get; set; }
        public string SAOSPatchVersion { get; set; }
    }
    public class OTITListReportModel
    {
        public string HardwareId { get; set; }
        public string Equipment { get; set; }
        public string Location { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string SerialNo { get; set; }
        public string WorkingCondition { get; set; }
        public string LastServiced { get; set; }
        public string Remark { get; set; }
        public string Owner { get; set; }
        public string PersonResponsible { get; set; }
        public string Criticality { get; set; }
        public string OperatingSystem { get; set; }
        public string OSPatchVersion { get; set; }
    }
    public class SoftwareAssetsReportModel
    {
        public string SoftwareId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string LicenseType { get; set; }
        public string Category { get; set; }
        public string IsActive { get; set; }
        public string Owner { get; set; }
        public string PersonResponsible { get; set; }
        public string Criticality { get; set; }
        public string OperatingSystem { get; set; }
        public string OSPatchVersion { get; set; }
    }
}
