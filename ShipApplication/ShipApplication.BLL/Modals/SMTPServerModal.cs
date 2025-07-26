using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class SMTPServerModal
    {
        public string SMPTServerName { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPFromAddress { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        public List<string> CCEmail { get; set; }
    }
}
