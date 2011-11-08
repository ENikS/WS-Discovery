using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace System.ServiceModel.Discovery.ProxyService.Host
{
    [RunInstaller(true)]
    public partial class FairwallInstaller : System.Configuration.Install.Installer
    {
        public FairwallInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
    }
}
