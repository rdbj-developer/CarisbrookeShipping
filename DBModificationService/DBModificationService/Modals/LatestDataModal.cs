using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Modals
{
    public class LatestDataModal
    {
        public long ID { get; set; }
        public long RecordID { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
    }
}
