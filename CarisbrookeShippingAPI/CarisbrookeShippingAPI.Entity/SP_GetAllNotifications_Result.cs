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
    
    public partial class SP_GetAllNotifications_Result
    {
        public string Name { get; set; }
        public string ReportType { get; set; }
        public string Deficiency { get; set; }
        public Nullable<System.Guid> DeficienciesUniqueID { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UserName { get; set; }
        public Nullable<int> CommentsCount { get; set; }
        public Nullable<int> ResolutionsCount { get; set; }
        public Nullable<int> InitialActionsCount { get; set; }
    }
}
