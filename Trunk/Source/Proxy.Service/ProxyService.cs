using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

#pragma warning disable 0649,0169

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// WCF service implementing WS-Discovery protocol.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class ProxyService : DiscoveryProxy
    {
        //-----------------------------------------------------
        //  Fields
        //-----------------------------------------------------

        #region Fields

        /// <summary>
        /// Collection of task factories for Online announcement. Upon receiving online announcement all of these tasks
        /// are created by calling each factory and passing <see cref="DiscoveryMessageSequence"/> and 
        /// <see cref="EndpointDiscoveryMetadata"/> to each factory. 
        /// </summary>
        [ImportMany]
        private IEnumerable<IAnounceOnlineTaskFactory> _onlineTaskFactories;

        /// <summary>
        /// Collection of task factories for Offline announcement. Upon receiving online announcement all of these tasks
        /// are created by calling each factory and passing <see cref="DiscoveryMessageSequence"/> and 
        /// <see cref="EndpointDiscoveryMetadata"/> to each factory. 
        /// </summary>
        [ImportMany]
        private IEnumerable<IAnounceOfflineTaskFactory> _offlineTaskFactories;

        [Import]
        private IProbeTaskFactory _findTaskFactory;

        [Import]
        private IResolveTaskFactory _resolveTaskFactory;

        #endregion

        //-----------------------------------------------------
        //  Online Announcements
        //-----------------------------------------------------

        #region OnlineAnnouncement

        /// <summary>
        /// Handles an online announcement message.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        /// <param name="callback">The callback delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state data.</param>
        /// <returns>A reference to the pending asynchronous operation.</returns>
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                  EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                  AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<object>(state);

            // Start a background task that will process anounncement
            Task.Factory.StartNew(() =>
            {
                // Create and executy all Online announcement tasks
                var tasks = _onlineTaskFactories.AsParallel()
                                                .Select((factory) => { return factory.Create(new DiscoveryMessageSequence[]{ messageSequence }, 
                                                                                             new EndpointDiscoveryMetadata[]{ endpointDiscoveryMetadata }); })
                                                .ToArray();
                // TODO: Decide if we want to wait for completion of all tasks
                // Task.WaitAll(tasks);
                tcs.SetResult(null);
                callback(tcs.Task);
            });
            
            return tcs.Task;
        }

        /// <summary>
        /// Handles the completion of an online announcement message.
        /// </summary>
        /// <param name="result">A reference to the completed asynchronous operation.</param>
        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();
        }

        #endregion

        //-----------------------------------------------------
        //  Offline Announcements
        //-----------------------------------------------------

        #region OfflineAnnouncement

        /// <summary>
        /// Handles an offline announcement message.
        /// </summary>
        /// <param name="messageSequence">The discovery message sequence.</param>
        /// <param name="endpointDiscoveryMetadata">The endpoint discovery metadata.</param>
        /// <param name="callback">The callback delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state data.</param>
        /// <returns>A reference to the pending asynchronous operation.</returns>
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence,
                                                                   EndpointDiscoveryMetadata endpointDiscoveryMetadata,
                                                                   AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<object>(state);

            // Start a background task that will process anounncement
            Task.Factory.StartNew(() =>
            {
                // Create and executy all Online announcement tasks
                var tasks = _offlineTaskFactories.AsParallel()
                                                 .Select((factory) => { return factory.Create(new DiscoveryMessageSequence[]{ messageSequence }, 
                                                                                              new EndpointDiscoveryMetadata[]{ endpointDiscoveryMetadata }); })
                                                 .ToArray();
                // TODO: Decide if we want to wait for completion of all tasks
                // Task.WaitAll(tasks);
                tcs.SetResult(null);
                callback(tcs.Task);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Handles the completion of an offline announcement.
        /// </summary>
        /// <param name="result">A reference to the completed asynchronous operation.</param>
        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();
        }

        #endregion

        //-----------------------------------------------------
        //  Probe
        //-----------------------------------------------------

        #region Find

        /// <summary>
        /// Handles a find operation.
        /// </summary>
        /// <param name="findRequestContext">The find request context that describes the service to discover.</param>
        /// <param name="callback">The callback delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state data.</param>
        /// <returns>A reference to the pending asynchronous operation.</returns>
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            return _findTaskFactory.Create(findRequestContext).ToApm(callback, state);
        }

        /// <summary>
        /// Handles the completion of a find operation.
        /// </summary>
        /// <param name="result">A reference to the completed asynchronous operation.</param>
        protected override void OnEndFind(IAsyncResult result)
        {
            if (!result.IsCompleted)
                result.AsyncWaitHandle.WaitOne();
        }

        #endregion

        //-----------------------------------------------------
        //  Resolve
        //-----------------------------------------------------

        #region Resolve

        /// <summary>
        /// Handles a resolve operation.
        /// </summary>
        /// <param name="resolveCriteria">The resolve criteria that describes the service to discover.</param>
        /// <param name="callback">The callback delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state data.</param>
        /// <returns>A reference to the pending asynchronous operation.</returns>
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            return _resolveTaskFactory.Create(resolveCriteria).ToApm(callback, state);
        }

        /// <summary>
        /// Handles the completion of a resolve operation.
        /// </summary>
        /// <param name="result">A reference to the completed asynchronous operation.</param>
        /// <returns>Endpoint discovery metadata for the resolved service.</returns>
        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return ((Task<EndpointDiscoveryMetadata>)result).Result;
        }

        #endregion

        //-----------------------------------------------------
        //  Redirect Adhoc to Managed Probe
        //-----------------------------------------------------

        #region ShouldRedirectFind

        // Summary:
        //     Override this method to allow the discovery proxy to send out multicast suppression
        //     messages when it receives a multicast find request.
        //
        // Parameters:
        //   resolveCriteria:
        //     The resolve criteria that describes the service to find.
        //
        //   callback:
        //     The callback delegate to call when the operation has completed.
        //
        //   state:
        //     The user-defined state data.
        //
        // Returns:
        //     A reference to the pending asynchronous operation.
        //protected override IAsyncResult BeginShouldRedirectFind(FindCriteria resolveCriteria, AsyncCallback callback, object state)
        //{
        //    return base.BeginShouldRedirectFind(resolveCriteria, callback, state);
        //}

        //
        // Summary:
        //     Override this method to handle the completion of sending the multicast suppression
        //     message for find requests.
        //
        // Parameters:
        //   result:
        //     A reference to the completed asynchronous operation.
        //
        //   redirectionEndpoints:
        //     A collection of endpoint discovery metadata that describes the redirection
        //     endpoints.
        //
        // Returns:
        //     true if the find operation should be redirected, otherwise false.
        //protected override bool EndShouldRedirectFind(IAsyncResult result, out Collection<EndpointDiscoveryMetadata> redirectionEndpoints)
        //{
        //    return base.EndShouldRedirectFind(result, out redirectionEndpoints);
        //}

        #endregion

        //-----------------------------------------------------
        //  Redirect Adhoc to Managed Resolve
        //-----------------------------------------------------

        #region ShouldRedirectResolve

        //
        // Summary:
        //     Override this method to allow the discovery proxy to send out multicast suppression
        //     messages when it receives a multicast resolve request.
        //
        // Parameters:
        //   findCriteria:
        //     The find criteria that describes the service to find.
        //
        //   callback:
        //     The callback delegate to call when the operation is completed.
        //
        //   state:
        //     The user-defined state data.
        //
        // Returns:
        //     A reference to the pending asynchronous operation.
        //protected override IAsyncResult BeginShouldRedirectResolve(ResolveCriteria findCriteria, AsyncCallback callback, object state)
        //{
        //    return base.BeginShouldRedirectResolve(findCriteria, callback, state);
        //}

        //
        // Summary:
        //     Override this method to handle the completion of sending the multicast suppression
        //     message for resolve requests.
        //
        // Parameters:
        //   result:
        //     A reference to the completed asynchronous operation.
        //
        //   redirectionEndpoints:
        //     A collection of endpoint discovery metadata that describes the redirection
        //     endpoints.
        //
        // Returns:
        //     true if the resolve operation should be redirected, otherwise false.
        //protected override bool EndShouldRedirectResolve(IAsyncResult result, out Collection<EndpointDiscoveryMetadata> redirectionEndpoints)
        //{
        //    return base.EndShouldRedirectResolve(result, out redirectionEndpoints);
        //}
        
        #endregion
    }
}
