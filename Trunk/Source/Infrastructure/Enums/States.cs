using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ServiceModel.Discovery
{
    public enum State
    {
        Unknown,
        ContainerCreated,
        ModulesInitializing,
        ModulesInitialized,
        DataLoading,
        DataLoaded
    }
}
