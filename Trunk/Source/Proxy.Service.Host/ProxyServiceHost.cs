using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Configuration;

namespace Proxy.Service.Host
{
    public partial class ProxyServiceHost : ServiceBase
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

        /// <summary>
        /// WCF service host
        /// </summary>
        private ServiceHost _proxyServiceHost;

        #endregion

        //-----------------------------------------------------
        //  Constructors
        //-----------------------------------------------------

        #region Constructors
        
        public ProxyServiceHost()
        {

            InitializeComponent();

            _proxyServiceHost = new ServiceHost(typeof(ProxyService), 
                                                new Uri(ConfigurationManager.AppSettings["BaseAddress"]  ?? 
                                                        "http://localhost:8732/Design_Time_Addresses/DiscoveryProxyService/"));
        }

        #endregion

        //-----------------------------------------------------
        //  Service Commands
        //-----------------------------------------------------

        #region Service Commands
        
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            OnStartProxyService();
        }

        protected override void OnStop()
        {
            base.OnStop();

            OnStopProxyService();
        }

        protected override void OnPause()
        {
            base.OnPause();

            OnStopProxyService();
        }

        protected override void OnContinue()
        {
            base.OnContinue();

            OnStartProxyService();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            OnStopProxyService();
        }
        
        #endregion

        //-----------------------------------------------------
        //  Private Methods
        //-----------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Starts WCF Proxy service
        /// </summary>
        private void OnStartProxyService()
        {
            try
            {
                _proxyServiceHost.Open();
            }
            catch (CommunicationException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                if (_proxyServiceHost.State != CommunicationState.Closed)
                {
                    Debug.WriteLine("Aborting the service...");
                    _proxyServiceHost.Abort();
                }
            }

        }

        /// <summary>
        /// Stops Proxy service
        /// </summary>
        private void OnStopProxyService()
        {
            _proxyServiceHost.Close();
        }

        #endregion
    }
}
