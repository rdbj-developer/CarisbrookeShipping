using System;
using System.Collections.Generic;

namespace ShipApplication.BLL.Modals
{
    public class CybersecurityRisksAssessmentModal
    {
        public CybersecurityRisksAssessmentForm CybersecurityRisksAssessmentForm { get; set; }
        public List<CybersecurityRisksAssessmentListModal> CybersecurityRisksAssessmentListModal { get; set; }
        public CybersecurityRisksAssessmentModal()
        {
            CybersecurityRisksAssessmentForm = new CybersecurityRisksAssessmentForm();
            CybersecurityRisksAssessmentListModal = new List<CybersecurityRisksAssessmentListModal>();
        }
    }
    public class CybersecurityRisksAssessmentForm
    {
        public Guid CRAId { get; set; }

        public string ShipName { get; set; }

        public string ShipCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsSynced { get; set; }
    }
    public class CybersecurityRisksAssessmentListModal
    {
        public Guid CRALId { get; set; }

        public Guid CRAId { get; set; }

        public string RiskId { get; set; }

        public string RiskDescription { get; set; }

        public string InherentRiskCategoryC { get; set; }
               
        public string InherentRiskCategoryI { get; set; }
               
        public string InherentRiskCategoryA { get; set; }
               
        public string InherentRiskCategoryS { get; set; }

        public string InherentImpactScore { get; set; }

        public string InherentLikelihoodScore { get; set; }

        public string InherentRiskScore { get; set; }

        public string RiskOwner { get; set; }

        public string Controls { get; set; }

        public string ResidualRiskCategoryC { get; set; }

        public string ResidualRiskCategoryI { get; set; }

        public string ResidualRiskCategoryA { get; set; }

        public string ResidualRiskCategoryS { get; set; }

        public string ResidualImpactScore { get; set; }

        public string ResidualLikelihoodScore { get; set; }

        public string ResidualRiskScore { get; set; }

        public string RiskDecision { get; set; }

        public string Vulnerability { get; set; }
        public string HardwareId { get; set; }
        public List<string> HardwareIdList { get; set; }
        public List<string> ControlsList { get; set; }
        public string InherentClass { get; set; }
        public string ResidualClass { get; set; }
        public bool IsUpdated { get; set; }
        public int ImpactScore { get; set; }
        public int ReseduleScore { get; set; }
    }
    public class CybersecurityRisksAssessmentReportModal
    {
        public string HardwareId { get; set; }
        public string RiskId { get; set; }
        public string RiskDescription { get; set; }
        public string Vulnerability { get; set; }
        public string InherentRiskCategoryC { get; set; }
        public string InherentRiskCategoryI { get; set; }
        public string InherentRiskCategoryA { get; set; }
        public string InherentRiskCategoryS { get; set; }
        public string InherentImpactScore { get; set; }
        public string InherentLikelihoodScore { get; set; }
        public string InherentRiskScore { get; set; }
        public string Controls { get; set; }
        public string ResidualRiskCategoryC { get; set; }
        public string ResidualRiskCategoryI { get; set; }
        public string ResidualRiskCategoryA { get; set; }
        public string ResidualRiskCategoryS { get; set; }
        public string ResidualImpactScore { get; set; }
        public string ResidualLikelihoodScore { get; set; }
        public string ResidualRiskScore { get; set; }
        public string RiskDecision { get; set; }
        public string RiskOwner { get; set; }
    }
}
