using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.ServiceModel.Discovery.Logging;
using System.Reactive.Subjects;
using System.IO;

namespace System.ServiceModel.Discovery
{
    /// <summary>
    /// The purpose of this part is to initialize MEF container,
    /// initialize all modules which implement Initialize contract and 
    /// satisfy all imports.
    /// </summary>
    [Export(typeof(ProxyService))]
    public partial class ProxyService : DiscoveryProxy
    {
        //-----------------------------------------------------
        //  Infrastructure Fields
        //-----------------------------------------------------

        #region Fields

        /// <summary>
        /// Gets or sets the default <see cref="CompositionContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="CompositionContainer"/> instance.</value>
        private CompositionContainer _container;


        #endregion

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProxyService()
            : base()
        {
            // Create and configure catalog
            AggregateCatalog catalog = ConfigureAggregateCatalog();

            // Register defaults if not yet registered by modules
            RegisterDefaultTypesIfMissing();
            
            // Create container
            _container = new CompositionContainer(catalog);
            if (_container == null)
                throw new InvalidOperationException();

            // Compose this
            _container.ComposeParts(this);
        }

        #endregion

        //-----------------------------------------------------
        //  Initialization
        //-----------------------------------------------------

        #region Initialization

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected virtual AggregateCatalog ConfigureAggregateCatalog()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            // Load Add-in modules from the directory
            if (Directory.Exists(ContractName.Modules)) 
                catalog.Catalogs.Add(new DirectoryCatalog(ContractName.Modules));

            // Add this assembly
            catalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));

            return catalog;
        }

        /// <summary>
        /// Helper method for configuring the <see cref="CompositionContainer"/>. 
        /// Registers defaults for all the types necessary for Prism to work, if they are not already registered.
        /// </summary>
        public virtual void RegisterDefaultTypesIfMissing()
        {
        }

        #endregion
    }
}
