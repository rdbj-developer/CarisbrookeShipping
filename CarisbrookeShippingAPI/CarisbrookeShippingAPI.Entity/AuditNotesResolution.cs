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
    
    public partial class AuditNotesResolution
    {
        public long ResolutionID { get; set; }
        public Nullable<long> AuditNoteID { get; set; }
        public string UserName { get; set; }
        public string Resolution { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.Guid> ResolutionUniqueID { get; set; }
        public Nullable<System.Guid> NotesUniqueID { get; set; }
        public Nullable<int> isNew { get; set; }
    }
}
