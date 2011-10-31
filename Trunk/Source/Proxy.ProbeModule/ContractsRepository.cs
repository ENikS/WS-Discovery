using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;

namespace Proxy.ProbeModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ContractsRepository
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
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        [Export(ContractName.OnlineAnnouncement)]
        public Task OnlineAnnouncementTaskFactory(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("AnnounceOnline task."));
        }

        /// <summary>
        /// Creates Task which processes Offline Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        [Export(ContractName.OfflineAnnouncement)]
        public Task OfflineAnnouncementTaskFactory(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("AnnounceOffline task."));
        }

        /// <summary>
        /// Creates a Task which processes Probe request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task"/> object which encapsulates request handler</returns>
        [Export(ContractName.Find, typeof(Func<FindRequestContext, Task>))]
        public Task FindTaskFactory(FindRequestContext findRequestContext)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("FindTaskFactory"));
        }

        #endregion
    }
}
