﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Proxy.Service.Host
{
    public partial class ServiceHost : ServiceBase
    {
        public ServiceHost()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }
    }
}
