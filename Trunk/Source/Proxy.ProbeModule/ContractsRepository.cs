using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using System.Reactive.Linq;

namespace Proxy.ProbeModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ContractsRepository : AnnouncementsBase, IProbeTaskFactory
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

        ConcurrentDictionary<XmlQualifiedName, IEnumerable<EndpointDiscoveryMetadata>> _contracts;

        #endregion

        //-----------------------------------------------------
        //  Constructors
        //-----------------------------------------------------

        #region Constructors

        public ContractsRepository()
        {
            _contracts = new ConcurrentDictionary<XmlQualifiedName, IEnumerable<EndpointDiscoveryMetadata>>();
        }

        #endregion

        //-----------------------------------------------------
        //  Private Methods
        //-----------------------------------------------------

        #region Methods

        private IEnumerable<EndpointDiscoveryMetadata> CreateCollectionFactory(XmlQualifiedName name)
        {
            return new ConcurrentBag<EndpointDiscoveryMetadata>();
        }

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
        Task IProbeTaskFactory.Create(FindRequestContext findRequestContext)
        {
            // Cancel the task once TimeOut passed
            CancellationTokenSource tokenSrc = new CancellationTokenSource();
            Observable.Interval(findRequestContext.Criteria.Duration).Take(1).Subscribe((i) => { tokenSrc.Cancel(); });

            return Task.Factory.StartNew(() => FindContract(findRequestContext, tokenSrc.Token), tokenSrc.Token);
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
            Parallel.ForEach(endpointDiscoveryMetadata.ContractTypeNames, (name)=>
            {
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _contracts.GetOrAdd(name, CreateCollectionFactory) 
                    as IProducerConsumerCollection<EndpointDiscoveryMetadata>;
                
                items.TryAdd(endpointDiscoveryMetadata);
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
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        override protected void OnOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                      EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            Parallel.ForEach(endpointDiscoveryMetadata.ContractTypeNames, (name) =>
            {
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _contracts.GetOrAdd(name, CreateCollectionFactory)
                    as IProducerConsumerCollection<EndpointDiscoveryMetadata>;
                
                items.TryTake(out endpointDiscoveryMetadata);

                if (0 == items.Count)
                {
                    IEnumerable<EndpointDiscoveryMetadata> value;

                    _contracts.TryRemove(name, out value);
                }
            });
        }

        #endregion

        //-----------------------------------------------------
        //  Probe/Find Request
        //-----------------------------------------------------

        #region Probe Request

        /// <summary>
        /// Handles Offline announcements.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        protected void FindContract(FindRequestContext findRequestContext, CancellationToken cancellationToken)
        {
            Parallel.ForEach(findRequestContext.Criteria.ContractTypeNames, (name) =>
            { 
                IEnumerable<EndpointDiscoveryMetadata> list;

                cancellationToken.ThrowIfCancellationRequested();

                if (_contracts.TryGetValue(name, out list))
                {
                    //var ser = list.AsParallel().Where((x) => { return true; });
                }
            });
        }

        #endregion
    }
}
