using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace System.ServiceModel.Discovery.ProxyService.Host
{
    [RunInstaller(true)]
    public partial class ProxyServiceHostInstaller : System.Configuration.Install.Installer
    {
        public ProxyServiceHostInstaller()
        {
            InitializeComponent();
        }
    }
}
