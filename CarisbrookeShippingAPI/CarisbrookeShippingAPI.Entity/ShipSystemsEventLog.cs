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
    
    public partial class ShipSystemsEventLog
    {
        public System.Guid Id { get; set; }
        public Nullable<long> ShipSystemId { get; set; }
        public Nullable<int> EventId { get; set; }
        public string EventSource { get; set; }
        public string EventDescription { get; set; }
        public Nullable<System.DateTime> EventDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UserName { get; set; }
        public string EventMachineName { get; set; }
        public string EventType { get; set; }
        public string EventLogType { get; set; }
    }
}
