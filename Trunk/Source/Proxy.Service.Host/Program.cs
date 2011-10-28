using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using ServiceProcess.Helpers;

namespace DiscoveryProxy.Host
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
                new DiscoveryProxyHost() 
            };
            //ServiceBase.Run(ServicesToRun);
            ServicesToRun.LoadServices(); 
        }
    }
}
