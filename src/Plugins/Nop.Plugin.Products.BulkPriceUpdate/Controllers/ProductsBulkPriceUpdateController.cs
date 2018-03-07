using Nop.Core;
using Nop.Plugin.Products.BulkPriceUpdate.Domain;
using Nop.Plugin.Products.BulkPriceUpdate.Infrastructure.Helpers;
using Nop.Plugin.Products.BulkPriceUpdate.Models;
using Nop.Plugin.Products.BulkPriceUpdate.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Products.BulkPriceUpdate.Controllers
{
    public class ProductsBulkPriceUpdateController : BasePluginController
    {
        private readonly BulkPriceUpdateSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IBulkPriceUpdateService _bulkPriceUpdateService;

        public ProductsBulkPriceUpdateController(BulkPriceUpdateSettings settings,
            ISettingService settingService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IVendorService vendorService,
            ICategoryService categoryService,
            IBulkPriceUpdateService bulkPriceUpdateService)
        {
            this._settings = settings;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._bulkPriceUpdateService = bulkPriceUpdateService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new BulkPriceUpdateConfigModel {
                EnableVerboseLogging = _settings.EnableVerboseLogging
            };

            return View("~/Plugins/Products.BulkPriceUpdate/Views/ProductsBulkPriceUpdate/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Configure(BulkPriceUpdateConfigModel model)
        {
            if (!ModelState.IsValid) {
                return Configure();
            }

            // save settings
            _settings.EnableVerboseLogging = model.EnableVerboseLogging;
            _settingService.SaveSetting(_settings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        
        public ActionResult BulkPriceUpdate()
        {
            var initial = BulkPriceUpdateByOptions.Vendor;
            var model = new BulkPriceUpdateModel {
                PriceUpdateBy = initial
            };

            model.AvailablePriceUpdateBy = SelectListHelper.GetPriceUpdateBy(initial);
            model.AvailableManufacturers = SelectListHelper.GetManufacturers(_manufacturerService, _localizationService);
            model.AvailableVendors = SelectListHelper.GetVendors(_vendorService, _localizationService);
            model.AvailableCategories = SelectListHelper.GetCategories(_categoryService, _localizationService);

            return View("~/Plugins/Products.BulkPriceUpdate/Views/ProductsBulkPriceUpdate/BulkPriceUpdate.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkPriceUpdate(BulkPriceUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BulkPriceUpdate();

            var updateById = (int?)null;
            switch (model.PriceUpdateBy) {
                case BulkPriceUpdateByOptions.Vendor:
                    updateById = model.Vendor;
                    break;
                case BulkPriceUpdateByOptions.Manufacturer:
                    updateById = model.Manufacturer;
                    break;
                case BulkPriceUpdateByOptions.Category:
                    updateById = model.Category;
                    break;
                default:
                    break;
            }

            if (updateById.HasValue)
                _bulkPriceUpdateService.UpdatePrices(model.PriceUpdateBy, updateById.Value, model.UpdateProductCosts,
                    model.ProductCostUpdateType, model.ProductCostUpdateValue, model.UpdatePrices, model.PriceUpdateType,
                    model.PriceUpdateValue, model.UpdateOldPrices);

            SuccessNotification(_localizationService.GetResource("Plugins.Products.BulkPriceUpdate.Saved"));

            // we return a new view, so the user's previous selections are persisted
            model.AvailablePriceUpdateBy = SelectListHelper.GetPriceUpdateBy(model.PriceUpdateBy);
            model.AvailableManufacturers = SelectListHelper.GetManufacturers(_manufacturerService, _localizationService);
            model.AvailableVendors = SelectListHelper.GetVendors(_vendorService, _localizationService);
            model.AvailableCategories = SelectListHelper.GetCategories(_categoryService, _localizationService);
            return View("~/Plugins/Products.BulkPriceUpdate/Views/ProductsBulkPriceUpdate/BulkPriceUpdate.cshtml", model);
        }
    }
}
