using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// Task factory interface to create Tasks handling Online announcements
    /// </summary>
    [InheritedExport]
    public interface IAnounceOnlineTaskFactory
    {
        /// <summary>
        /// Creates Task to handle Online announcement
        /// </summary>
        /// <param name="messageSequence">Array of <see cref="messageSequence"/> headers</param>
        /// <param name="endpointDiscoveryMetadata">Array of <see cref="EndpointDiscoveryMetadata"/> endpoints</param>
        /// <returns>Returns Task object which encapsulates handler</returns>
        Task Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata);
    }

    /// <summary>
    /// Task factory interface to create Tasks handling Offline announcements
    /// </summary>
    [InheritedExport]
    public interface IAnounceOfflineTaskFactory
    {
        /// <summary>
        /// Crates Task to handle offline announcement
        /// </summary>
        /// <param name="messageSequence">Array of <see cref="messageSequence"/> headers</param>
        /// <param name="endpointDiscoveryMetadata">Array of <see cref="EndpointDiscoveryMetadata"/> endpoints</param>
        /// <returns>Returns Task object which encapsulates handler</returns>
        Task Create(DiscoveryMessageSequence[] messageSequence, EndpointDiscoveryMetadata[] endpointDiscoveryMetadata);
    }
}
