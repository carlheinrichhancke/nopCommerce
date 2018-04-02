using Nop.Plugin.Products.BulkPriceUpdate.Domain;
using Nop.Plugin.Products.BulkPriceUpdate.Infrastructure.ModelBinders;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Products.BulkPriceUpdate.Models
{
    [ModelBinder(typeof(DecimalModelBinder))]
    public class BulkPriceUpdateModel : BaseNopModel
    {
        public BulkPriceUpdateModel()
        {
            AvailablePriceUpdateBy = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Products.BulkPriceUpdate.UpdateBy")]
        public BulkPriceUpdateByOptions PriceUpdateBy { get; set; }
        public IList<SelectListItem> AvailablePriceUpdateBy { get; set; }

        public int Manufacturer { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public int Vendor { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        public int Category { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        [NopResourceDisplayName("Plugins.Products.BulkPriceUpdate.Cost")]
        public bool UpdateProductCosts { get; set; }

        [NopResourceDisplayName("Plugins.Products.BulkPriceUpdate.Price")]
        public bool UpdatePrices { get; set; }
        
        public BulkPriceUpdateTypes ProductCostUpdateType { get; set; }
        public decimal ProductCostUpdateValue { get; set; }

        public BulkPriceUpdateTypes PriceUpdateType { get; set; }
        public decimal PriceUpdateValue { get; set; }

        [NopResourceDisplayName("Plugins.Products.BulkPriceUpdate.UpdateOldPrice")]
        public bool UpdateOldPrices { get; set; }
    }
}
