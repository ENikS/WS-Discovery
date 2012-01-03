using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Proxy.Service.Host
{
    [RunInstaller(true)]
    public partial class ProxyServiceInstaller : System.Configuration.Install.Installer
    {
        public ProxyServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
