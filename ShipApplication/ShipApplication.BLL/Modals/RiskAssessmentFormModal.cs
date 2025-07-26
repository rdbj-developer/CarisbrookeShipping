using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{

    public class RiskAssessmentFormModal
    {
        public RiskAssessmentForm RiskAssessmentForm { get; set; }
        public List<RiskAssessmentFormHazard> RiskAssessmentFormHazardList { get; set; }
        public List<RiskAssessmentFormReviewer> RiskAssessmentFormReviewerList { get; set; }
        public RiskAssessmentFormModal()
        {
            RiskAssessmentForm = new RiskAssessmentForm();
            RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>();
            RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>();
        }
    }

    public class RiskAssessmentForm
    {
        public long RAFID { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 11/26/2022
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
        public int Issue { get; set; }
        public DateTime? IssueDate { get; set; }
        public int Amendment { get; set; }
        public DateTime? AmendmentDate { get; set; }
        public bool? IsConfidential { get; set; }
        public Guid? DocumentID { get; set; }
        public Guid? ParentID { get; set; }
        public string Type { get; set; }
        public string SectionType { get; set; }
        public bool? IsAmended { get; set; }
        public bool? IsApplicable { get; set; }
    }

    public class RiskAssessmentFormHazard
    {
        public Guid Id { get; set; }
        public long? HazardId { get; set; }
        public long? RAFID { get; set; }
        public string Stage1Description { get; set; }
        public string Stage2Severity { get; set; }
        public string Stage2Likelihood { get; set; }
        public string Stage2RiskFactor { get; set; }
        public string Stage3Description { get; set; }
        public string Stage4Severity { get; set; }
        public string Stage4Likelihood { get; set; }
        public string Stage4RiskFactor { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 11/26/2022

    }

    public class RiskAssessmentFormReviewer
    {
        public Guid Id { get; set; }
        public long? RAFID { get; set; }
        public string ReviewerName { get; set; }
        public long? IndexNo { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 11/26/2022
    }

    public class RiskFactorColorClass
    {
        public static List<RiskFactorColorClass> RiskFactorColorList;
        public RiskFactorColorClass()
        {
            RiskFactorColorList = new List<RiskFactorColorClass>();
            RiskFactorColorList.Add(new RiskFactorColorClass
            {
                Severity = "Slight",
                Likelihood = "Very Unlikely",
                RiskFactor = "Very Low Risk",
                Color = "bg-green-active"
            });
            RiskFactorColorList.Add(new RiskFactorColorClass
            {
                Severity = "Slight",
                Likelihood = "Very Unlikely",
                RiskFactor = "Very Low Risk",
                Color = "bg-green-active"
            });
        }
        public string Severity { get; set; }
        public string Likelihood { get; set; }
        public string RiskFactor { get; set; }
        public string Color { get; set; }
    }

    public class RiskAssessmentFormListModal
    {
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
        public List<RiskAssessmentFormHazard> RiskAssessmentFormHazardLists { get; set; }
        public List<RiskAssessmentFormReviewer> RiskAssessmentFormReviewerLists { get; set; }
    }

    public class RiskAssessmentReviewLog
    {
        public string ShipName { get; set; }    // JSL 06/22/2022
        public string ShipCode { get; set; }    // JSL 06/22/2022
        public string Number { get; set; }
        public string Title { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? AmendmentDate { get; set; }
        public string Stage4RiskFactor { get; set; }
        public string RiskFactorColour { get; set; }
        public bool? IsApplicable { get; set; }
        public DateTime? ReviewerDate { get; set; }
        public long RAFID { get; set; }
        public Guid? RAFUniqueID { get; set; } // JSL 11/26/2022
    }
    public class RiskAssessmentDataList
    {
        public List<RiskAssessmentForm> RiskAssessmentList { get; set; }
        public List<RiskAssessmentFormHazard> RiskAssessmentFormHazardList { get; set; }
        public List<RiskAssessmentFormReviewer> RiskAssessmentFormReviewerList { get; set; }
    }
}
