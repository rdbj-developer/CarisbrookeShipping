using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    // JSL 06/23/2022 added this file
    public class UserHubModels
    {
        public string UserName { get; set; }
        public DateTime InitDateTime { get; set; }  // JSL 06/30/2022
        public HashSet<string> ConnectionIds { get; set; }
    }
}
