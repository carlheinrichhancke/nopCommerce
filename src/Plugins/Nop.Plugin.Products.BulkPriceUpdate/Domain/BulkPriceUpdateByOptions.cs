using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Products.BulkPriceUpdate.Domain
{
    /// <summary>
    /// Represents the available targets to update prices by
    /// </summary>
    public enum BulkPriceUpdateByOptions
    {
        Vendor = 0,
        Manufacturer = 1,
        Category = 2
    }
}
