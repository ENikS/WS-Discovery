using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;

namespace Proxy.ProbeModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ContractsRepository : AnnouncementsBase, IFindTaskFactory
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
        /// Creates Task to handle Probe request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task"/> object which encapsulates request handler</returns>
        Task IFindTaskFactory.Create(FindRequestContext findRequestContext)
        {
            return Task.Factory.StartNew(() => Debug.WriteLine("FindTaskFactory"));
        }

        #endregion

        //-----------------------------------------------------
        //  Online Announcements
        //-----------------------------------------------------

        #region OnlineAnnouncement

        /// <summary>
        /// Handles Online announcements.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        override protected void OnOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                     EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
        }

        #endregion

        //-----------------------------------------------------
        //  Offline Announcements
        //-----------------------------------------------------

        #region OfflineAnnouncement

        /// <summary>
        /// Handles Offline announcements.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        override protected void OnOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                      EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
        }

        #endregion

    }
}
