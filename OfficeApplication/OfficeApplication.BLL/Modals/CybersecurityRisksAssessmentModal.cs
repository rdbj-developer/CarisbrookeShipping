using System;
using System.Collections.Generic;

namespace OfficeApplication.BLL.Modals
{
    public class CybersecurityRisksAssessmentModal
    {
        public CybersecurityRisksAssessmentForm CybersecurityRisksAssessmentForm { get; set; }
        public List<CybersecurityRisksAssessmentListModal> CybersecurityRisksAssessmentListModal { get; set; }
        public bool IsFromOfficeApp { get; set; }
        public List<string> HardwareListBoxValues { get; set; }        
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
        public Guid RemovedCRALId { get; set; }
    }
    public class CybersecurityRisksAssessmentListModal
    {
        public Guid CRALId { get; set; }

        public Guid CRAId { get; set; }

        public string RiskId { get; set; }

        public string RiskDescription { get; set; }

        public bool? InherentRiskCategoryC { get; set; }

        public bool? InherentRiskCategoryI { get; set; }

        public bool? InherentRiskCategoryA { get; set; }

        public bool? InherentRiskCategoryS { get; set; }

        public string InherentImpactScore { get; set; }

        public string InherentLikelihoodScore { get; set; }

        public string InherentRiskScore { get; set; }

        public string RiskOwner { get; set; }

        public string Controls { get; set; }

        public bool? ResidualRiskCategoryC { get; set; }

        public bool? ResidualRiskCategoryI { get; set; }

        public bool? ResidualRiskCategoryA { get; set; }

        public bool? ResidualRiskCategoryS { get; set; }

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
        public bool? InherentRiskCategoryC { get; set; }
        public bool? InherentRiskCategoryI { get; set; }
        public bool? InherentRiskCategoryA { get; set; }
        public bool? InherentRiskCategoryS { get; set; }
        public string InherentImpactScore { get; set; }
        public string InherentLikelihoodScore { get; set; }
        public string InherentRiskScore { get; set; }
        public string Controls { get; set; }
        public bool? ResidualRiskCategoryC { get; set; }
        public bool? ResidualRiskCategoryI { get; set; }
        public bool? ResidualRiskCategoryA { get; set; }
        public bool? ResidualRiskCategoryS { get; set; }
        public string ResidualImpactScore { get; set; }
        public string ResidualLikelihoodScore { get; set; }
        public string ResidualRiskScore { get; set; }
        public string RiskDecision { get; set; }
        public string RiskOwner { get; set; }
    }
    public class CyberSecuritySettingsModal
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class CyberSecurityCopyDataModal
    {
        public string SourceShipCode { get; set; }
        public string DestinationShipCode { get; set; }
        public List<CybersecurityRisksAssessmentListModal> RiskAssessList { get; set; }
    }
}
