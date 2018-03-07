using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Products.BulkPriceUpdate
{
    public class BulkPriceUpdateSettings : ISettings
    {
        /// <summary>
        /// Gets or sets whether verbose logging is enabled for the plugin.
        /// </summary>
        public bool EnableVerboseLogging { get; set; }
    }
}
