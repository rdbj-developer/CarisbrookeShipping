using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Modals
{
    //RDBJ 11/08/2021 Created this class
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
