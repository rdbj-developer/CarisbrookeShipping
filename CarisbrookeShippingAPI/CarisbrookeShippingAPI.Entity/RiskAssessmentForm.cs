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
    
    public partial class RiskAssessmentForm
    {
        public long RAFID { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string ReviewerName { get; set; }
        public Nullable<System.DateTime> ReviewerDate { get; set; }
        public string ReviewerRank { get; set; }
        public string ReviewerLocation { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> SavedAsDraft { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public string Code { get; set; }
        public Nullable<int> Issue { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<int> Amendment { get; set; }
        public Nullable<System.DateTime> AmendmentDate { get; set; }
        public Nullable<bool> IsConfidential { get; set; }
        public Nullable<System.Guid> DocumentID { get; set; }
        public Nullable<System.Guid> ParentID { get; set; }
        public string Type { get; set; }
        public string SectionType { get; set; }
        public Nullable<bool> IsAmended { get; set; }
        public Nullable<bool> IsApplicable { get; set; }
        public Nullable<System.Guid> RAFUniqueID { get; set; }
    }
}
