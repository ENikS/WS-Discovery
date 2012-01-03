using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Diagnostics;


namespace Proxy.Service.Host
{
    [RunInstaller(true)]
    public partial class AppConfigInstaller : System.Configuration.Install.Installer
    {
        private const string _multicast = "Multicast";
        
        public AppConfigInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            if (!stateSaver.Contains(_multicast))
                stateSaver.Add(_multicast, (object)true);

        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);

            ConfigurationSectionGroup serviceModelGroup = config.SectionGroups["system.serviceModel"];
            ConfigurationSection serviceSection = serviceModelGroup.Sections["services"];
            ServiceElement proxyService = (serviceSection as ServicesSection).Services["System.ServiceModel.Discovery.ProxyService"];

            // Add Endpoints
            proxyService.Endpoints.Add(new ServiceEndpointElement() { Name = "UdpMulticastEndpoint", IsSystemEndpoint = false, Kind = "udpAnnouncementEndpoint" });

            Debug.WriteLine("Section Groups : ");
            foreach (var service in (serviceSection as ServicesSection).Services)
            {
                Debug.WriteLine(service);
            }

            config.Save();
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }
    }
}
