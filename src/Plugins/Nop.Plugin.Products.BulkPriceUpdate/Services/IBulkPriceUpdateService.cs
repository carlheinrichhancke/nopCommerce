using Nop.Plugin.Products.BulkPriceUpdate.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Products.BulkPriceUpdate.Services
{
    public interface IBulkPriceUpdateService
    {
        /// <summary>
        /// Performs the bulk prices updates
        /// </summary>
        /// <param name="updateBy">The grouping of products to update by</param>
        /// <param name="groupById">The ID of the grouping to retrieve the products by</param>
        /// <param name="updateProdCost">Indicates whether Product Costs should be updated</param>
        /// <param name="prodCostUpdateType">The type of Product Cost update to perform (percentage\flat amount)</param>
        /// <param name="prodCostValue">The value with which to update the Product Costs</param>
        /// <param name="updatePrice">Indicates whether Prices should be updated</param>
        /// <param name="priceUpdateType">The type of Price update to perform (percentage\flat amount)</param>
        /// <param name="priceValue">The value with which to update the Prices</param>
        /// <param name="updateOldPrice">Indicates whether the Old Price field should be updated with the previous price</param>
        void UpdatePrices(BulkPriceUpdateByOptions updateBy,
            int groupById,
            bool updateProdCost,
            BulkPriceUpdateTypes prodCostUpdateType,
            decimal prodCostValue,
            bool updatePrice,
            BulkPriceUpdateTypes priceUpdateType,
            decimal priceValue,
            bool updateOldPrice);
    }
}
