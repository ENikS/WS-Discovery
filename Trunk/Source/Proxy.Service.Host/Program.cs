using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
#if DEBUG
using ServiceProcess.Helpers;
#endif

namespace Proxy.Service.Host
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
                new ProxyServiceHost() 
            };
#if DEBUG
            ServicesToRun.LoadServices();
#else
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
