using System.ServiceProcess;

namespace AWS_DB_UpdateService
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

            //Service1 obj = new Service1();
            //obj.StartSync();

            //CheckStatusService objCheckStatusService = new CheckStatusService();
            //objCheckStatusService.StartStatusCheck();
        }
    }
}
