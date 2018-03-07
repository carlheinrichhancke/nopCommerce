using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Products.BulkPriceUpdate.Models
{
    public class BulkPriceUpdateConfigModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Products.BulkPriceUpdate.Fields.EnableVerboseLogging")]
        public bool EnableVerboseLogging { get; set; }
        public bool EnableVerboseLogging_OverrideForStore { get; set; }
    }
}
