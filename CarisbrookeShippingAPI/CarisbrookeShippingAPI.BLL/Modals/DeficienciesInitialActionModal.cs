using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class GIRDeficienciesInitialActions //RDBJ 09/22/2021 Rename Class Name
    {
        public System.Guid? IniActUniqueID { get; set; }
        public Nullable<System.Guid> DeficienciesUniqueID { get; set; }
        public GIRDeficienciesInitialActions()
        {
            GIRDeficienciesInitialActionsFiles = new List<GIRDeficienciesInitialActionsFile>();
        }
        public long GIRInitialID { get; set; }
        public Nullable<long> DeficienciesID { get; set; }
        public Nullable<long> GIRFormID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public List<GIRDeficienciesInitialActionsFile> GIRDeficienciesInitialActionsFiles { get; set; }
        public int? isNew { get; set; } //RDBJ 10/16/2021
    }

    public class GIRDeficienciesInitialActionsFile
    {
        public System.Guid? IniActUniqueID { get; set; }
        public System.Guid? IniActFileUniqueID { get; set; }
        public long GIRFileID { get; set; }
        public long? GIRInitialID { get; set; }
        public long? DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
