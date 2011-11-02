using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// Base class for types that wish to receive On/Offline announcements
    /// </summary>
    [InheritedExport]
    public abstract class AnnouncementsBase<T> : IAnounceOnlineTaskFactory, IAnounceOfflineTaskFactory, IPartImportsSatisfiedNotification 
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

        /// <summary>
        /// Internal storage
        /// </summary>
        protected readonly ConcurrentDictionary<T, IEnumerable<EndpointDiscoveryMetadata>> _dictionary;

        /// <summary>
        /// Delegate that creates Endpoint collection (the Value) for each Key
        /// </summary>
        [Import(AllowDefault = true, AllowRecomposition = true)]
        protected Func<T, IEnumerable<EndpointDiscoveryMetadata>> _endpointCollectionFactory;

        /// <summary>
        /// Predicate determining if two collections of <see cref="XElement"/> objects intersect.
        /// </summary>
        [Import(AllowDefault = true, AllowRecomposition = true)]
        protected Func<Collection<XElement>, Collection<XElement>, bool> _extensionsPredicate;

        #endregion

        //-----------------------------------------------------
        //  Constructors
        //-----------------------------------------------------

        #region Constructors

        public AnnouncementsBase()
        {
            _dictionary = new ConcurrentDictionary<T, IEnumerable<EndpointDiscoveryMetadata>>();
        }

        #endregion

        //-----------------------------------------------------
        //  Abstract Methods
        //-----------------------------------------------------

        #region Abstract Methods

        /// <summary>
        /// Handles Online announcements.
        /// </summary>
        /// <param name="messageSequence">The array discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The array of endpoint discovery metadata.</param>
        protected abstract void OnOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                    EndpointDiscoveryMetadata endpointDiscoveryMetadata);
        /// <summary>
        /// Handles Offline announcements.
        /// </summary>
        /// <param name="messageSequence">The array discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The array of endpoint discovery metadata.</param>
        protected abstract void OnOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                     EndpointDiscoveryMetadata endpointDiscoveryMetadata);

        #endregion
        
        //-----------------------------------------------------
        //  Virtual Methods
        //-----------------------------------------------------

        #region Virtual Methods

        /// <summary>
        /// This methos is called once all available Imports have
        /// been satisfied. If some imports are not resolved this
        /// method allowd to provide defaults.
        /// </summary>
        protected virtual void OnSatisfiedImports()
        {
            // Init default collection factory if required
            if (null == _endpointCollectionFactory)
                _endpointCollectionFactory = (k) => { return new ConcurrentBag<EndpointDiscoveryMetadata>(); };

            // Init default extensions predicate
            if (null == _extensionsPredicate)
                _extensionsPredicate = ExtensionsPredicate;
        }

        /// <summary>
        /// Default predicate comparint extension collections
        /// </summary>
        /// <param name="endpointExtension">Collection of extensions of the endpoint</param>
        /// <param name="criteriaExtension">Collection of requested extensions</param>
        /// <returns>Returns True if collections match</returns>
        protected virtual bool ExtensionsPredicate(Collection<XElement> endpointExtension, Collection<XElement> criteriaExtension)
        {
            // TODO: Add proper match algorithm from standard docs

            return true;
        }

        #endregion

        //-----------------------------------------------------
        //  Interface Implementations
        //-----------------------------------------------------

        #region IAnounceOnlineTaskFactory Members

        /// <summary>
        /// Creates Task that handles Online Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        Task IAnounceOnlineTaskFactory.Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() =>
            {
                if (null == endpointDiscoveryMetadata)
                    throw new ArgumentNullException("EndpointDiscoveryMetadata");
            
                Parallel.For(0, endpointDiscoveryMetadata.Length, (i) =>
                {
                    OnOnlineAnnouncement((null == messageSequence) ? null : messageSequence[i], endpointDiscoveryMetadata[i]);
                });
            });
        }

        #endregion

        #region IAnounceOfflineTaskFactory Members

        /// <summary>
        /// Creates Task that handles Offline Announcement message.
        /// </summary>
        /// <param name="messageSequence"><see cref="DiscoveryMessageSequence"/> header info</param>
        /// <param name="endpointDiscoveryMetadata"><see cref="EndpointDiscoveryMetadata"/> information</param>
        /// <returns>Returns <see cref="Task"/> object encapsulating message handler</returns>
        Task IAnounceOfflineTaskFactory.Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata)
        {
            return Task.Factory.StartNew(() => 
            {
                if (null == endpointDiscoveryMetadata)
                    throw new ArgumentNullException("EndpointDiscoveryMetadata");

                Parallel.For(0, endpointDiscoveryMetadata.Length, (i) =>
                {
                    OnOfflineAnnouncement((null == messageSequence) ? null : messageSequence[i], endpointDiscoveryMetadata[i]);
                });
            });
        }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        /// <summary>
        /// This interface is called once all available Imports have
        /// been satisfied.
        /// </summary>
        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            OnSatisfiedImports();
        }

        #endregion
    }
}
