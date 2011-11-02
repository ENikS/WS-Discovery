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
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Reactive.Concurrency;

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

        private IEnumerable<EndpointDiscoveryMetadata> EndpointCollectionFactory(XmlQualifiedName name)
        {
            return new ConcurrentBag<EndpointDiscoveryMetadata>();
        }

        #endregion

        //-----------------------------------------------------
        //  Probe Task factory
        //-----------------------------------------------------

        #region Task Factories

        /// <summary>
        /// Creates Task to handle Probe request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task"/> object which encapsulates request handler</returns>
        Task IProbeTaskFactory.Create(FindRequestContext findRequestContext)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            // Create Observable and Subscribe to it
            findRequestContext.Criteria.ContractTypeNames
                                       .ToObservable()
                                       .ObserveOn(Scheduler.NewThread)                                                  // Asynchronously
                                       .TakeUntil(Observable.Return(XmlQualifiedName.Empty)                             // Take until...
                                       .Delay(findRequestContext.Criteria.Duration))                                    // ...until timeout
                                       .Where(name => { return _contracts.ContainsKey(name); })                         // Select only contract names present in the dictionary
                                       .SelectMany(name => { return _contracts[name].ToObservable(); })                 // Endpoints implementing requested Contract name
                                       .Where(endpoint =>
                                       {
                                           return ScopesPredicate(endpoint.Scopes,                                      // With matching scopes
                                                                  findRequestContext.Criteria.Scopes,
                                                                  findRequestContext.Criteria.ScopeMatchBy)

                                               && ExtensionsPredicate(endpoint.Extensions,                              // and matching extensions  
                                                                      findRequestContext.Criteria.Extensions);
                                       })
                                       .Take(findRequestContext.Criteria.MaxResults)                                    // Take requested number of results
                                       .Subscribe(onNext: x => findRequestContext.AddMatchingEndpoint(x),               // Add matching endpoint is found
                                                  onError: ex => tcs.TrySetException(ex),                               // Report error if any
                                                  onCompleted: () => tcs.SetResult(null));                              // Complete task
            return tcs.Task;
        }

        private bool ScopesPredicate(Collection<Uri> endpointScopes, Collection<Uri> criteriaScopes, Uri scopeMatchBy)
        {
            // if no scopes requested endpint matches
            if (0 == criteriaScopes.Count)
                return true;

            // TODO: Add proper match algorithm from standard docs

            return true;
        }

        private bool ExtensionsPredicate(Collection<XElement> endpointExtension, Collection<XElement> criteriaExtension)
        {
            // TODO: Add proper match algorithm from standard docs

            return true;
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
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _contracts.GetOrAdd(name, EndpointCollectionFactory) 
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
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _contracts.GetOrAdd(name, EndpointCollectionFactory)
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
    }
}
