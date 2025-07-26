using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<bool> IsReport { get; set; }
        public Nullable<bool> IsDocument { get; set; }
        public Nullable<bool> IsForm { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<bool> IsSettings { get; set; }
        public Nullable<bool> IsHome { get; set; }
        public Nullable<bool> IsDeficiencies { get; set; }
        public Nullable<bool> IsHelp { get; set; }
        public Nullable<bool> IsAbout { get; set; }
    }
}
