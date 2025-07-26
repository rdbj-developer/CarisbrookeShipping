using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class RepositoryModal
    {
        public long RepID { get; set; }
        public string DocumentID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public Nullable<double> Version { get; set; }
    }
}
