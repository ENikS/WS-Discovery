using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// Base class for types that wish to receive On/Offline announcements
    /// </summary>
    [InheritedExport]
    public abstract class AnnouncementsBase : IAnounceOnlineTaskFactory, IAnounceOfflineTaskFactory
    {
        //-----------------------------------------------------
        //  Abstract members
        //-----------------------------------------------------
        
        #region Abstracts

        /// <summary>
        /// Handles Online announcements.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        protected abstract void OnOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                    EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        
        /// <summary>
        /// Handles Offline announcements.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        protected abstract void OnOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                     EndpointDiscoveryMetadata endpointDiscoveryMetadata);

        #endregion

        //-----------------------------------------------------
        //  Task factories
        //-----------------------------------------------------

        #region Task Factories


        /// <summary>
        /// Creates Task that handles Online Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        Task IAnounceOnlineTaskFactory.Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => OnOnlineAnnouncement(messageSequence, endpointDiscoveryMetadata));
        }

        /// <summary>
        /// Creates Task that handles Offline Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        Task IAnounceOfflineTaskFactory.Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => OnOfflineAnnouncement(messageSequence, endpointDiscoveryMetadata));
        }

        #endregion

        //-----------------------------------------------------
        //  Online Announcements
        //-----------------------------------------------------

        #region OnlineAnnouncement

        /// <summary>
        /// Handles Online announcements.
        /// </summary>
        /// <param name="messageSequence">The array discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The array of endpoint discovery metadata.</param>
        protected virtual void OnOnlineAnnouncement(DiscoveryMessageSequence[] messageSequence,
                                                    EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            if (null == endpointDiscoveryMetadata)
                throw new ArgumentNullException("EndpointDiscoveryMetadata");
            
            Parallel.For(0, endpointDiscoveryMetadata.Length, (i) =>
            {
                OnOnlineAnnouncement((null == messageSequence) ? null : messageSequence[i], endpointDiscoveryMetadata[i]);
            });
        }

        #endregion

        //-----------------------------------------------------
        //  Offline Announcements
        //-----------------------------------------------------

        #region OfflineAnnouncement

        /// <summary>
        /// Handles Offline announcements.
        /// </summary>
        /// <param name="messageSequence">The array discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The array of endpoint discovery metadata.</param>
        protected virtual void OnOfflineAnnouncement(DiscoveryMessageSequence[] messageSequence,
                                                     EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            if (null == endpointDiscoveryMetadata)
                throw new ArgumentNullException("EndpointDiscoveryMetadata");

            Parallel.For(0, endpointDiscoveryMetadata.Length, (i) => 
            {
                OnOfflineAnnouncement((null == messageSequence) ? null : messageSequence[i], endpointDiscoveryMetadata[i]);
            });
        }

        #endregion
    }
}
