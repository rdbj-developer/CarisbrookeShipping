using System;

namespace AWS_PO_UpdateService.Modals
{
    public class CodafinPOModal
    {
        public string cmpcode { get; set; }
        public string doccode { get; set; }
        public string docnum { get; set; }
        public DateTime? moddate { get; set; }
        public string el1 { get; set; }
        public decimal? valuedoc { get; set; }
        public string descr { get; set; }
        public string invoice { get; set; }
    }
}
