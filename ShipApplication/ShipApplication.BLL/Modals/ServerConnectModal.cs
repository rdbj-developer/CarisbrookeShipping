﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class ServerConnectModal
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public bool IsConnection { get; set; }
        public bool IsDBCreated { get; set; }
        public bool IsInspector { get; set; }
    }
}
