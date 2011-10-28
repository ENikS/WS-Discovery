using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Discovery
{
    public static class ContractName
    {
        public const string Initialize = "Initialize";                                   // typeof(Action)
        
        public const string OnlineAnnouncement = "OnlineAnnouncementContract";           // typeof(Func<DiscoveryMessageSequence, EndpointDiscoveryMetadata, Task>)
        public const string OfflineAnnouncement = "OfflineAnnouncementContract";         // typeof(Func<DiscoveryMessageSequence, EndpointDiscoveryMetadata, Task>)

        public const string Find = "FindContract";                                       // typeof(Func<FindCriteria, Task>)
        public const string Probe = "ProbeContract";                                     // typeof(Func<FindRequestContext, Task>)
        public const string Resolve = "ProbeContract";                                   // typeof(Func<ResolveCriteria, Task<EndpointDiscoveryMetadata>>)
    }
}
