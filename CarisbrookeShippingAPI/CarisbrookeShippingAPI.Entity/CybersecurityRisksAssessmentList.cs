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
    
    public partial class CybersecurityRisksAssessmentList
    {
        public System.Guid CRALId { get; set; }
        public System.Guid CRAId { get; set; }
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
    }
}
