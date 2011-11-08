using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Proxy.ProbeModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ContractsRepository : AnnouncementsBase<XmlQualifiedName>, IProbeTaskFactory
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

        /// <summary>
        /// Predicate determining if requested scopes are within available scopes
        /// </summary>
        [Import(AllowDefault = true, AllowRecomposition = true)]
        private Func<Collection<Uri>, Collection<Uri>, Uri, bool> _scopesPredicate;

        #endregion

        //-----------------------------------------------------
        //  Private Methods
        //-----------------------------------------------------

        #region Methods

        /// <summary>
        /// This methos is called once all available Imports have
        /// been satisfied. If some imports are not resolved this
        /// method allowd to provide defaults.
        /// </summary>
        protected override void OnSatisfiedImports()
        {
            base.OnSatisfiedImports();

            // Init default scopes predicate
            if (null == _scopesPredicate)
                _scopesPredicate = ScopesPredicate;
        }

        /// <summary>
        /// Default implementation of Scopes predicate
        /// </summary>
        /// <param name="endpointScopes">Collection of scopes of the endpoint</param>
        /// <param name="criteriaScopes">Collection of requested scopes</param>
        /// <param name="scopeMatchBy">Matching algorithm</param>
        /// <returns>Returns True if Scopes match</returns>
        private bool ScopesPredicate(Collection<Uri> endpointScopes, Collection<Uri> criteriaScopes, Uri scopeMatchBy)
        {
            // if no scopes requested endpint matches
            if (0 == criteriaScopes.Count)
                return true;

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
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _dictionary.GetOrAdd(name, _endpointCollectionFactory) 
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
                if (!_dictionary.ContainsKey(name))
                    return;
                
                IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _dictionary[name]
                    as IProducerConsumerCollection<EndpointDiscoveryMetadata>;
                
                items.TryTake(out endpointDiscoveryMetadata);

                if (0 == items.Count)
                {
                    IEnumerable<EndpointDiscoveryMetadata> value;

                    _dictionary.TryRemove(name, out value);
                }
            });
        }

        #endregion

        //-----------------------------------------------------
        //  Interface Implementations
        //-----------------------------------------------------

        #region IProbeTaskFactory Members

        /// <summary>
        /// Creates Task to handle Probe request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task"/> object which encapsulates request handler</returns>
        Task IProbeTaskFactory.Create(FindRequestContext findRequestContext)
        {
            // Create Task containing Rx LINQ query 
            return findRequestContext.Criteria
                                     .ContractTypeNames                                                       // Each requested contract name
                                     .ToObservable()                                                          // As Observable 
                                     .ObserveOn(Scheduler.NewThread)                                          // Asynchronously
                                     .TakeUntil(Observable.Return(XmlQualifiedName.Empty)                     // Take until...
                                         .Delay(findRequestContext.Criteria.Duration))                        // ...until timeout
                                     .Where(name => { return _dictionary.ContainsKey(name); })                // Select only contract names present in the dictionary
                                     .SelectMany(name => { return _dictionary[name].ToObservable(); })        // Endpoints implementing requested Contract name
                                     .Where(endpoint =>
                                     {
                                         return _scopesPredicate(endpoint.Scopes,                             // With matching scopes
                                                                 findRequestContext.Criteria.Scopes,
                                                                 findRequestContext.Criteria.ScopeMatchBy)

                                             && _extensionsPredicate(endpoint.Extensions,                     // and matching extensions  
                                                                     findRequestContext.Criteria.Extensions);
                                     })
                                     .Take(findRequestContext.Criteria.MaxResults)                            // Take requested number of results
                                     .Aggregate(findRequestContext,
                                                (context, endpoint) =>
                                                {
                                                    context.AddMatchingEndpoint(endpoint);                    // Add matching endpoints
                                                    return context;
                                                })
                                     .ToTask();
        }

        /// <summary>
        /// Creates Task to handle Probe request
        /// </summary>
        /// <param name="criteria"><see cref="FindCriteria"/> for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task<IEnumerable<EndpointDiscoveryMetadata>>"/> object which encapsulates request handler</returns>
        Task<Collection<EndpointDiscoveryMetadata>> IProbeTaskFactory.Create(FindCriteria criteria)
        {
            // Create Task containing Rx LINQ query 
            return criteria.ContractTypeNames                                                       // Each requested contract name
                           .ToObservable()                                                          // As Observable 
                           .ObserveOn(Scheduler.NewThread)                                          // Asynchronously
                           .TakeUntil(Observable.Return(XmlQualifiedName.Empty)                     // Take until...
                               .Delay(criteria.Duration))                                           // ...until timeout
                           .Where(name => { return _dictionary.ContainsKey(name); })                // Select only contract names present in the dictionary
                           .SelectMany(name => { return _dictionary[name].ToObservable(); })        // Endpoints implementing requested Contract name
                           .Where(endpoint =>
                           {
                               return _scopesPredicate(endpoint.Scopes,                             // With matching scopes
                                                       criteria.Scopes,
                                                       criteria.ScopeMatchBy)

                                   && _extensionsPredicate(endpoint.Extensions,                     // and matching extensions  
                                                           criteria.Extensions);
                           })
                           .Take(criteria.MaxResults)                                               // Take requested number of results
                           .Aggregate(new Collection<EndpointDiscoveryMetadata>(),
                                   (context, endpoint) =>
                                   {
                                       context.Add(endpoint);                                       // Add matching endpoints
                                       return context;
                                   })
                           .ToTask();
        }

        #endregion
    }
}
