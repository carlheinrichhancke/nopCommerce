using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Products.BulkPriceUpdate.Domain
{
    /// <summary>
    /// Represents the available types by which prices can be updated
    /// </summary>
    public enum BulkPriceUpdateTypes
    {
        Perc = 0, // update by percentage
        Flat = 1 // update by flat value
    }
}
