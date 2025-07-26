using System.ServiceProcess;

namespace AWS_PO_UpdateService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
            //Service1 service1 = new Service1();
            //service1.StartSync();
        }
    }
}
