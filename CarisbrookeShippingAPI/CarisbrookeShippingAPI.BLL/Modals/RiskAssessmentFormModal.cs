using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class RiskAssessmentFormModal
    {
        public RiskAssessmentForm RiskAssessmentForm { get; set; }
        public List<RiskAssessmentFormHazard> RiskAssessmentFormHazardList { get; set; }
        public List<RiskAssessmentFormReviewer> RiskAssessmentFormReviewerList { get; set; }
    }

    public class RAFDetailViewRequestModal {
        public string id { get; set; }  // JSL 11/20/2022 set string instead of int
        public string ShipName { get; set; }
    }
    public class RiskAssessmentReviewLog
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? AmendmentDate { get; set; }
        public string Stage4RiskFactor { get; set; }
        public string RiskFactorColour { get; set; }
        public bool? IsApplicable { get; set; }
        public DateTime? ReviewerDate { get; set; }
        public long RAFID { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 11/20/2022
    }
    public class RiskAssessmentDataList
    {
        public List<RiskAssessmentForm> RiskAssessmentList { get; set; }
        public List<RiskAssessmentFormHazard> RiskAssessmentFormHazardList { get; set; }
        public List<RiskAssessmentFormReviewer> RiskAssessmentFormReviewerList { get; set; }
        public RiskAssessmentDataList()
        {
            RiskAssessmentList = new List<RiskAssessmentForm>();
            RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>();
            RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>();
        }
    }

    // JSL 11/26/2022
    public class RiskAssessmentFormDetails
    {
        public Nullable<System.Guid> RAFUniqueID { get; set; } 
        public long RAFID { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string ReviewerName { get; set; }
        public DateTime? ReviewerDate { get; set; }
        public string ReviewerRank { get; set; }
        public string ReviewerLocation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? SavedAsDraft { get; set; }
        public bool? IsSynced { get; set; }
        public string Code { get; set; }
        public int? Issue { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? Amendment { get; set; }
        public DateTime? AmendmentDate { get; set; }
        public bool? IsConfidential { get; set; }
        public Guid? DocumentID { get; set; }
        public Guid? ParentID { get; set; }
        public string Type { get; set; }
        public string SectionType { get; set; }
        public bool? IsAmended { get; set; }
        public bool? IsApplicable { get; set; }
    }
    // End JSL 11/26/2022
}
