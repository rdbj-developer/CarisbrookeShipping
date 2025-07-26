using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class GIRDeficienciesResolution //RDBJ 09/22/2021 Rename Class Name
    {
        public System.Guid? ResolutionUniqueID { get; set; } //RDBJ 09/22/2021
        public Nullable<System.Guid> DeficienciesUniqueID { get; set; } //RDBJ 09/22/2021
        public long GIRResolutionID { get; set; }
        public Nullable<long> DeficienciesID { get; set; }
        public long GIRFormID { get; set; }
        public string Name { get; set; }
        public string Resolution { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public List<GIRDeficienciesResolutionFile> GIRDeficienciesResolutionFiles { get; set; }
        public GIRDeficienciesResolution()
        {
            GIRDeficienciesResolutionFiles = new List<GIRDeficienciesResolutionFile>();
        }
        public int? isNew { get; set; } //RDBJ 10/16/2021
    }
    public class GIRDeficienciesResolutionFile
    {
        public System.Guid? ResolutionUniqueID { get; set; } //RDBJ 09/22/2021
        public System.Guid? ResolutionFileUniqueID { get; set; } //RDBJ 09/22/2021
        public long GIRFileID { get; set; }
        public long? GIRResolutionID { get; set; }
        public long? DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
