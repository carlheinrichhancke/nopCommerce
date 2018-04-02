using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Products.BulkPriceUpdate.Domain;
using Nop.Services.Catalog;
using Nop.Services.Logging;

namespace Nop.Plugin.Products.BulkPriceUpdate.Services
{
    public class BulkPriceUpdateService : IBulkPriceUpdateService
    {
        private readonly BulkPriceUpdateSettings _settings;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public BulkPriceUpdateService(BulkPriceUpdateSettings settings,
            IProductService productService,
            ILogger logger)
        {
            this._settings = settings;
            this._productService = productService;
            this._logger = logger;
        }

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
        public void UpdatePrices(BulkPriceUpdateByOptions updateBy,
            int groupById,
            bool updateProdCost,
            BulkPriceUpdateTypes prodCostUpdateType,
            decimal prodCostValue,
            bool updatePrice,
            BulkPriceUpdateTypes priceUpdateType,
            decimal priceValue,
            bool updateOldPrice)
        {
            if (updateProdCost)
                UpdateProductCosts(updateBy, groupById, prodCostUpdateType, prodCostValue);

            if (updatePrice)
                UpdatePrices(updateBy, groupById, priceUpdateType, priceValue, updateOldPrice);
        }

        private void UpdateProductCosts(BulkPriceUpdateByOptions updateBy,
            int groupById,
            BulkPriceUpdateTypes updateType,
            decimal value)
        {
            var products = GetProducts(updateBy, groupById);

            if (_settings.EnableVerboseLogging)
                _logger.Information($"Total number of products to update Product Costs: {products.TotalCount}");
            
            foreach (var prod in products)
                prod.ProductCost = GetUpdatedValue(updateType, prod.ProductCost, value);

            _productService.UpdateProducts(products);

            if (_settings.EnableVerboseLogging)
                _logger.Information($"Product Costs successfully updated for {products.TotalCount} products");
        }

        private void UpdatePrices(BulkPriceUpdateByOptions updateBy,
            int groupById,
            BulkPriceUpdateTypes updateType,
            decimal value,
            bool updateOldPrice)
        {
            var products = GetProducts(updateBy, groupById);

            if (_settings.EnableVerboseLogging)
                _logger.Information($"Total number of products to update Prices: {products.TotalCount}");

            foreach (var prod in products) {
                if (updateOldPrice)
                    prod.OldPrice = prod.Price;

                prod.Price = GetUpdatedValue(updateType, prod.Price, value);
            }

            _productService.UpdateProducts(products);

            if (_settings.EnableVerboseLogging)
                _logger.Information($"Prices successfully updated for {products.TotalCount} products");
        }

        private IPagedList<Product> GetProducts(BulkPriceUpdateByOptions updateBy, int groupById)
        {
            IPagedList<Product> result = null;

            switch (updateBy) {
                case BulkPriceUpdateByOptions.Vendor:
                    result = _productService.SearchProducts(vendorId: groupById);
                    break;
                case BulkPriceUpdateByOptions.Manufacturer:
                    result = _productService.SearchProducts(manufacturerId: groupById);
                    break;
                case BulkPriceUpdateByOptions.Category:
                    result = _productService.SearchProducts(categoryIds: new List<int> { groupById });
                    break;
                default:
                    break;
            }

            return result;
        }

        private decimal GetUpdatedValue(BulkPriceUpdateTypes updateType, decimal oldValue, decimal updateValue)
        {
            var result = 0m;

            switch (updateType) {
                case BulkPriceUpdateTypes.Perc:
                    result = oldValue * (updateValue / 100m + 1);
                    break;
                case BulkPriceUpdateTypes.Flat:
                    result = oldValue + updateValue;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
