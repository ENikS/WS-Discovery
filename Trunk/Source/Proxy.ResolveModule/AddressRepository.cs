using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;

namespace Proxy.ResolveModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AddressRepository
    {
        //-----------------------------------------------------
        //  Methods
        //-----------------------------------------------------

        #region Methods


        #endregion

        //-----------------------------------------------------
        //  Task factories
        //-----------------------------------------------------

        #region Task Factories

        /// <summary>
        /// Creates Task which processes Online Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns><see cref="Task"/> object encapsulating message handler</returns>
        [Export(ContractName.OnlineAnnouncement, typeof(Func<DiscoveryMessageSequence, EndpointDiscoveryMetadata, Task>))]
        public Task OnlineAnnouncementTaskFactory(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("AnnounceOnline task."));
        }

        /// <summary>
        /// Creates Task which processes Offline Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns><see cref="Task"/> object encapsulating message handler</returns>
        [Export(ContractName.OfflineAnnouncement, typeof(Func<DiscoveryMessageSequence, EndpointDiscoveryMetadata, Task>))]
        public Task OfflineAnnouncementTaskFactory(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("AnnounceOffline task."));
        }

        /// <summary>
        /// Creates a Task which processes Resolve request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task<EndpointDiscoveryMetadata>"/> object which encapsulates request handler</returns>
        [Export(ContractName.Resolve, typeof(Func<ResolveCriteria, Task<EndpointDiscoveryMetadata>>))]
        public Task<EndpointDiscoveryMetadata> ResolveTaskFactory(ResolveCriteria resolveCriteria)
        {
            return Task<EndpointDiscoveryMetadata>.Factory.StartNew(() => { return null; });
        }

        #endregion
    }
}
