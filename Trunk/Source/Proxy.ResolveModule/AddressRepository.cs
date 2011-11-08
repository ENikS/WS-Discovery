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
using System.ServiceModel;

namespace Proxy.ResolveModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AddressRepository : AnnouncementsBase<EndpointAddress>, IResolveTaskFactory
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

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
            IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _dictionary.GetOrAdd(endpointDiscoveryMetadata.Address, _endpointCollectionFactory)
                as IProducerConsumerCollection<EndpointDiscoveryMetadata>;

            items.TryAdd(endpointDiscoveryMetadata);
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
            if (!_dictionary.ContainsKey(endpointDiscoveryMetadata.Address))
                return;
            
            IProducerConsumerCollection<EndpointDiscoveryMetadata> items = _dictionary[endpointDiscoveryMetadata.Address]
                as IProducerConsumerCollection<EndpointDiscoveryMetadata>;

            items.TryTake(out endpointDiscoveryMetadata);

            if (0 == items.Count)
            {
                IEnumerable<EndpointDiscoveryMetadata> value;

                _dictionary.TryRemove(endpointDiscoveryMetadata.Address, out value);
            }
        }

        #endregion

        //-----------------------------------------------------
        //  Interface Implementations
        //-----------------------------------------------------

        #region IResolveTaskFactory Members

        /// <summary>
        /// Creates a Task that processes Resolve request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task<EndpointDiscoveryMetadata>"/> object which encapsulates request handler</returns>
        Task<Collection<EndpointDiscoveryMetadata>> IResolveTaskFactory.Create(ResolveCriteria resolveCriteria)
        {
            if (!_dictionary.ContainsKey(resolveCriteria.Address))
            { 
                var source = new TaskCompletionSource<Collection<EndpointDiscoveryMetadata>>();
                source.SetResult(new Collection<EndpointDiscoveryMetadata>());

                return source.Task;
            }
            
            // Create Task containing Rx LINQ query 
            return _dictionary[resolveCriteria.Address].ToObservable()                                               // As Observable 
                                                       .ObserveOn(Scheduler.NewThread)                               // Asynchronously
                                                       .TakeUntil(Observable.Return((EndpointDiscoveryMetadata)null) // Take until...
                                                       .Delay(resolveCriteria.Duration))                             // ...until timeout
                                                       .Where(endpoint =>
                                                       {
                                                           return _extensionsPredicate(endpoint.Extensions,          // Match extensions  
                                                                                       resolveCriteria.Extensions);
                                                       })
                                                       .Take(1)                                                      // TODO: Findout if more than one required
                                                       .Aggregate(new Collection<EndpointDiscoveryMetadata>(),
                                                               (context, endpoint) =>
                                                               {
                                                                   context.Add(endpoint);                            // Add matching endpoints
                                                                   return context;
                                                               })
                                                       .ToTask();
        }

        #endregion
    }
}
