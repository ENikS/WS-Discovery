using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;

namespace Proxy.ResolveModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AddressRepository : AnnouncementsBase, IResolveTaskFactory
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
        /// Creates a Task that processes Resolve request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task<EndpointDiscoveryMetadata>"/> object which encapsulates request handler</returns>
        Task<EndpointDiscoveryMetadata> IResolveTaskFactory.Create(ResolveCriteria resolveCriteria)
        {
            return Task<EndpointDiscoveryMetadata>.Factory.StartNew(() => { return null; });
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
