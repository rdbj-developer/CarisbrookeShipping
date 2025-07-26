
namespace CarisbrookeShippingService.BLL.Modals
{
    public class ServerConnectModal
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public bool IsConnection { get; set; }
        public bool IsDBCreated { get; set; }
        public bool IsInspector { get; set; }   // JSL 11/12/2022
    }
}
