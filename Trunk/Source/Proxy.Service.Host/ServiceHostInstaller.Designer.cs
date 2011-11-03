﻿namespace System.ServiceModel.Discovery.ProxyService.Host
{
    partial class ServiceHostInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProxyServiceHost = new Proxy.Service.Host.ServiceHost();
            // 
            // ProxyServiceHost
            // 
            this.ProxyServiceHost.CanShutdown = true;
            this.ProxyServiceHost.ExitCode = 0;
            this.ProxyServiceHost.ServiceName = "WSDiscoveryProxy";

        }

        #endregion

        private Proxy.Service.Host.ServiceHost ProxyServiceHost;
    }
}