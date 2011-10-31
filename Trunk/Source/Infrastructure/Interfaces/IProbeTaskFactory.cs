using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// Task factory interface to create Tasks handling Probe requests
    /// </summary>
    [InheritedExport(ContractName.Find)]
    public interface IProbeTaskFactory
    {
        /// <summary>
        /// Creates Task to handle Probe request
        /// </summary>
        /// <param name="findRequestContext"><see cref="FindRequestContext"/> request info.</param>
        /// <returns>Returns encapsulating Task object</returns>
        Task Create(FindRequestContext findRequestContext);
    }
}
