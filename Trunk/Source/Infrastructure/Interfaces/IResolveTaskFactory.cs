using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// Task factory interface to create Tasks handling Resolve requests
    /// </summary>
    [InheritedExport]
    public interface IResolveTaskFactory
    {
        /// <summary>
        /// Creates a Task which processes Resolve request
        /// </summary>
        /// <param name="findRequestContext">Criteria for finding correct endpoints</param>
        /// <returns>Returns <see cref="Task<EndpointDiscoveryMetadata>"/> object which encapsulates request handler</returns>
        Task<Collection<EndpointDiscoveryMetadata>> Create(ResolveCriteria resolveCriteria);
    }
}
