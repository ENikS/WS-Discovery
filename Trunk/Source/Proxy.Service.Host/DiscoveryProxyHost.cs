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
using System.ServiceModel.Description;

namespace DiscoveryProxy.Host
{
    public partial class DiscoveryProxyHost : ServiceBase
    {
        #region Fields

        // Host the DiscoveryProxy service
        private ServiceHost _proxyServiceHost;

        #endregion

        public DiscoveryProxyHost()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            // Host the DiscoveryProxy service
            _proxyServiceHost = null;
            //_proxyServiceHost = new ServiceHost(typeof(Proxy), new Uri("http://localhost:8732/Design_Time_Addresses/DiscoveryProxyService/"));

            try
            {
                //_proxyServiceHost.AddDefaultEndpoints();

                // Make the service discoverable over UDP multicast
                ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
                discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
                _proxyServiceHost.Description.Behaviors.Add(discoveryBehavior);
                
                //// Add DiscoveryEndpoint to receive Probe and Resolve messages
                //UdpDiscoveryEndpoint discoveryEndpoint = new UdpDiscoveryEndpoint();
                //discoveryEndpoint.IsSystemEndpoint = false;
                //_proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                UdpAnnouncementEndpoint announcementEndpoint = new UdpAnnouncementEndpoint();
                _proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                // Add Metadata behavior
                //ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior() { HttpGetEnabled = true };
                //_proxyServiceHost.Description.Behaviors.Add(metadataBehavior);
                //_proxyServiceHost.AddServiceEndpoint(new ServiceMetadataEndpoint()
                //{ 
                //    IsSystemEndpoint = true,
                //    Address = new EndpointAddress("http://localhost:8732/Design_Time_Addresses/DiscoveryProxyService/mex") 
                //});

                //_proxyServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                //                                     MetadataExchangeBindings.CreateMexHttpBinding(),
                //                                     "mex");
                //ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior() { HttpGetEnabled = true };
                //_proxyServiceHost.Description.Behaviors.Add(metadataBehavior);
                //_proxyServiceHost.AddServiceEndpoint(typeof(IMetadataExchange),
                //                                     MetadataExchangeBindings.CreateMexHttpBinding(),
                //                                     "http://localhost:8732/Design_Time_Addresses/DiscoveryProxyService/mex");

            
                foreach (ServiceEndpoint se in _proxyServiceHost.Description.Endpoints)
                {
                    Debug.WriteLine("Endpoint details:");
                    Debug.WriteLine("Logical address: \t{0}", se.Address);
                    Debug.WriteLine("Physical address: \t{0}", se.ListenUri);
                    Debug.WriteLine("Binding: \t{0}", se.Binding.Name);
                    Debug.WriteLine("Contract: \t{0}", se.Contract.Name);
                    Debug.WriteLine("");
                }

                _proxyServiceHost.Open();
            }
            catch (CommunicationException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            if (_proxyServiceHost.State != CommunicationState.Closed)
            {
                Debug.WriteLine("Aborting the service...");
                _proxyServiceHost.Abort();
            }
            
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }

        protected override void OnStop()
        {
            _proxyServiceHost.Close();
        }
    }
}
