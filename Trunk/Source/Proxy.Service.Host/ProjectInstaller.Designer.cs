namespace System.ServiceModel.Discovery.ProxyService.Host
{
    partial class ProjectInstaller
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
            this.ProxyProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ProxyServiceHostInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ProxyProcessInstaller
            // 
            this.ProxyProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ProxyProcessInstaller.Password = null;
            this.ProxyProcessInstaller.Username = null;
            // 
            // ProxyServiceHostInstaller
            // 
            this.ProxyServiceHostInstaller.Description = "WS-Discovery Proxy WCF service";
            this.ProxyServiceHostInstaller.DisplayName = "WS-Discovery Proxy";
            this.ProxyServiceHostInstaller.ServiceName = "WSDiscoveryProxy";
            this.ProxyServiceHostInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ProxyProcessInstaller,
            this.ProxyServiceHostInstaller});

        }

        #endregion

        private ServiceProcess.ServiceProcessInstaller ProxyProcessInstaller;
        private ServiceProcess.ServiceInstaller ProxyServiceHostInstaller;
    }
}