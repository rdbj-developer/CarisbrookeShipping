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
    
    public partial class CB_FormsPersonMaster
    {
        public decimal Id { get; set; }
        public System.Guid UniqueId { get; set; }
        public string PersonName { get; set; }
        public int PersonType { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
