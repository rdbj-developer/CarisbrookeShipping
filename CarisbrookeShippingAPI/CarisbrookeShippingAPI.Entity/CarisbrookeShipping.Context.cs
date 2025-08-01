﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class CarisbrookeShippingEntities : DbContext
    {
        public CarisbrookeShippingEntities()
            : base("name=CarisbrookeShippingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ACCOUNTCODE> ACCOUNTCODEs { get; set; }
        public DbSet<AuditNote> AuditNotes { get; set; }
        public DbSet<AuditNotesAttachment> AuditNotesAttachments { get; set; }
        public DbSet<AuditNotesComment> AuditNotesComments { get; set; }
        public DbSet<AuditNotesCommentsFile> AuditNotesCommentsFiles { get; set; }
        public DbSet<CSShip> CSShips { get; set; }
        public DbSet<GeneralInspectionReport> GeneralInspectionReports { get; set; }
        public DbSet<GIRDeficiency> GIRDeficiencies { get; set; }
        public DbSet<GIRDeficienciesCommentFile> GIRDeficienciesCommentFiles { get; set; }
        public DbSet<GIRDeficienciesFile> GIRDeficienciesFiles { get; set; }
        public DbSet<GIRDeficienciesNote> GIRDeficienciesNotes { get; set; }
        public DbSet<GIRPhotograph> GIRPhotographs { get; set; }
        public DbSet<GIRRestandWorkHour> GIRRestandWorkHours { get; set; }
        public DbSet<GlRCrewDocument> GlRCrewDocuments { get; set; }
        public DbSet<GlRSafeManningRequirement> GlRSafeManningRequirements { get; set; }
        public DbSet<OpexReportFinilizeDate> OpexReportFinilizeDates { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<SIRAdditionalNote> SIRAdditionalNotes { get; set; }
        public DbSet<SIRNote> SIRNotes { get; set; }
        public DbSet<SuperintendedInspectionReport> SuperintendedInspectionReports { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VIMSLogInfomation> VIMSLogInfomations { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<UserGroupMenuPermission> UserGroupMenuPermissions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<ShipSystem> ShipSystems { get; set; }
        public DbSet<ShipSystemsEventLog> ShipSystemsEventLogs { get; set; }
        public DbSet<ShipSystemsInfo> ShipSystemsInfoes { get; set; }
        public DbSet<ShipSystemsProcess> ShipSystemsProcesses { get; set; }
        public DbSet<ShipSystemsService> ShipSystemsServices { get; set; }
        public DbSet<HoldVentilationRecordForm> HoldVentilationRecordForms { get; set; }
        public DbSet<HoldVentilationRecordSheet> HoldVentilationRecordSheets { get; set; }
        public DbSet<ShipAppReleaseNote> ShipAppReleaseNotes { get; set; }
        public DbSet<ShipAppDownloadLog> ShipAppDownloadLogs { get; set; }
        public DbSet<FeedbackForm> FeedbackForms { get; set; }
        public DbSet<SMRForm> SMRForms { get; set; }
        public DbSet<SMRFormCrewMember> SMRFormCrewMembers { get; set; }
        public DbSet<RiskAssessmentForm> RiskAssessmentForms { get; set; }
        public DbSet<RiskAssessmentFormHazard> RiskAssessmentFormHazards { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<MainSyncServicesEventLog> MainSyncServicesEventLogs { get; set; }
        public DbSet<OpenFileServicesEventLog> OpenFileServicesEventLogs { get; set; }
        public DbSet<ArrivalReport> ArrivalReports { get; set; }
        public DbSet<DailyCargoReport> DailyCargoReports { get; set; }
        public DbSet<DailyPositionReport> DailyPositionReports { get; set; }
        public DbSet<DepartureReport> DepartureReports { get; set; }
        public DbSet<RiskAssessmentFormReviewer> RiskAssessmentFormReviewers { get; set; }
        public DbSet<CodaPurchaseOrder> CodaPurchaseOrders { get; set; }
        public DbSet<AssetManagmentEquipmentITList> AssetManagmentEquipmentITLists { get; set; }
        public DbSet<AssetManagmentEquipmentList> AssetManagmentEquipmentLists { get; set; }
        public DbSet<AssetManagmentEquipmentOTList> AssetManagmentEquipmentOTLists { get; set; }
        public DbSet<AssetManagmentEquipmentSoftwareAsset> AssetManagmentEquipmentSoftwareAssets { get; set; }
        public DbSet<AssetManagmentEquipmentLog> AssetManagmentEquipmentLogs { get; set; }
        public DbSet<CybersecurityRisksAssessment> CybersecurityRisksAssessments { get; set; }
        public DbSet<CybersecurityRisksAssessmentList> CybersecurityRisksAssessmentLists { get; set; }
        public DbSet<CyberSecuritySetting> CyberSecuritySettings { get; set; }
        public DbSet<ShipSystemsSoftwareInfo> ShipSystemsSoftwareInfoes { get; set; }
        public DbSet<CybersecurityRisksAssessmentLog> CybersecurityRisksAssessmentLogs { get; set; }
        public DbSet<OpenFileServicesDownloadLog> OpenFileServicesDownloadLogs { get; set; }
        public DbSet<OpenFileServicesReleaseNote> OpenFileServicesReleaseNotes { get; set; }
        public DbSet<RiskAssessmentFormLog> RiskAssessmentFormLogs { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<GIRDeficienciesInitialActionsFile> GIRDeficienciesInitialActionsFiles { get; set; }
        public DbSet<GIRDeficienciesResolution> GIRDeficienciesResolutions { get; set; }
        public DbSet<GIRDeficienciesResolutionFile> GIRDeficienciesResolutionFiles { get; set; }
        public DbSet<AuditNotesResolution> AuditNotesResolutions { get; set; }
        public DbSet<AuditNotesResolutionFile> AuditNotesResolutionFiles { get; set; }
        public DbSet<GIRDeficienciesInitialAction> GIRDeficienciesInitialActions { get; set; }
        public DbSet<HelpAndSupport> HelpAndSupports { get; set; }
        public DbSet<InternalAuditForm> InternalAuditForms { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MLCRegulationTree> MLCRegulationTrees { get; set; }
        public DbSet<SMSReferencesTree> SMSReferencesTrees { get; set; }
        public DbSet<SSPReferenceTree> SSPReferenceTrees { get; set; }
        public DbSet<CB_FormsPersonMaster> CB_FormsPersonMaster { get; set; }
        public DbSet<FSTOInspection> FSTOInspections { get; set; }
        public DbSet<FSTOInspectionAttachment> FSTOInspectionAttachments { get; set; }
    
        public virtual ObjectResult<SP_GetNotificationDetailsById_Result> SP_GetNotificationDetailsById(string deficienciesUniqueID, string formType, string crntUserName)
        {
            var deficienciesUniqueIDParameter = deficienciesUniqueID != null ?
                new ObjectParameter("DeficienciesUniqueID", deficienciesUniqueID) :
                new ObjectParameter("DeficienciesUniqueID", typeof(string));
    
            var formTypeParameter = formType != null ?
                new ObjectParameter("formType", formType) :
                new ObjectParameter("formType", typeof(string));
    
            var crntUserNameParameter = crntUserName != null ?
                new ObjectParameter("crntUserName", crntUserName) :
                new ObjectParameter("crntUserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetNotificationDetailsById_Result>("SP_GetNotificationDetailsById", deficienciesUniqueIDParameter, formTypeParameter, crntUserNameParameter);
        }
    
        public virtual ObjectResult<SP_Get_GIDeficiencies_OR_SIActionableItems_Number_Result> SP_Get_GIDeficiencies_OR_SIActionableItems_Number(string ship, string reportType, Nullable<int> min, Nullable<int> max, string uniqueFormID)
        {
            var shipParameter = ship != null ?
                new ObjectParameter("Ship", ship) :
                new ObjectParameter("Ship", typeof(string));
    
            var reportTypeParameter = reportType != null ?
                new ObjectParameter("ReportType", reportType) :
                new ObjectParameter("ReportType", typeof(string));
    
            var minParameter = min.HasValue ?
                new ObjectParameter("min", min) :
                new ObjectParameter("min", typeof(int));
    
            var maxParameter = max.HasValue ?
                new ObjectParameter("max", max) :
                new ObjectParameter("max", typeof(int));
    
            var uniqueFormIDParameter = uniqueFormID != null ?
                new ObjectParameter("UniqueFormID", uniqueFormID) :
                new ObjectParameter("UniqueFormID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_GIDeficiencies_OR_SIActionableItems_Number_Result>("SP_Get_GIDeficiencies_OR_SIActionableItems_Number", shipParameter, reportTypeParameter, minParameter, maxParameter, uniqueFormIDParameter);
        }
    
        public virtual int SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(string uniqueFormID)
        {
            var uniqueFormIDParameter = uniqueFormID != null ?
                new ObjectParameter("UniqueFormID", uniqueFormID) :
                new ObjectParameter("UniqueFormID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501", uniqueFormIDParameter);
        }
    
        public virtual ObjectResult<SP_GetAllNotifications_Result> SP_GetAllNotifications(string userID, Nullable<int> userGroup, string crntUserName)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            var userGroupParameter = userGroup.HasValue ?
                new ObjectParameter("UserGroup", userGroup) :
                new ObjectParameter("UserGroup", typeof(int));
    
            var crntUserNameParameter = crntUserName != null ?
                new ObjectParameter("crntUserName", crntUserName) :
                new ObjectParameter("crntUserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetAllNotifications_Result>("SP_GetAllNotifications", userIDParameter, userGroupParameter, crntUserNameParameter);
        }
    
        public virtual ObjectResult<SP_GetAssignedToMeGISIIADeficiencies_Result> SP_GetAssignedToMeGISIIADeficiencies(string userID)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetAssignedToMeGISIIADeficiencies_Result>("SP_GetAssignedToMeGISIIADeficiencies", userIDParameter);
        }
    
        public virtual ObjectResult<SP_GetNotificationsForTheISMDashboard_Result> SP_GetNotificationsForTheISMDashboard(string userID)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetNotificationsForTheISMDashboard_Result>("SP_GetNotificationsForTheISMDashboard", userIDParameter);
        }
    
        public virtual ObjectResult<SP_GetAllNotificationsByUser_Result> SP_GetAllNotificationsByUser(string userID)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetAllNotificationsByUser_Result>("SP_GetAllNotificationsByUser", userIDParameter);
        }
    
        public virtual ObjectResult<CB_proc_GetRecentNotifications_Result> CB_proc_GetRecentNotifications(string userID, Nullable<System.DateTime> createdDateTime)
        {
            var userIDParameter = userID != null ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(string));
    
            var createdDateTimeParameter = createdDateTime.HasValue ?
                new ObjectParameter("CreatedDateTime", createdDateTime) :
                new ObjectParameter("CreatedDateTime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<CB_proc_GetRecentNotifications_Result>("CB_proc_GetRecentNotifications", userIDParameter, createdDateTimeParameter);
        }
    }
}
