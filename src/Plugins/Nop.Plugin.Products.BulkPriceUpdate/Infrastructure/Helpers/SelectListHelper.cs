using Nop.Core;
using Nop.Plugin.Products.BulkPriceUpdate.Domain;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Products.BulkPriceUpdate.Infrastructure.Helpers
{
    public static class SelectListHelper
    {
        public static IList<SelectListItem> GetPriceUpdateBy(BulkPriceUpdateByOptions initial)
        {
            var result = new List<SelectListItem>();
            
            foreach (BulkPriceUpdateByOptions option in Enum.GetValues(typeof(BulkPriceUpdateByOptions))) {
                result.Add(new SelectListItem {
                    Text = CommonHelper.ConvertEnum(option.ToString()),
                    Value = ((int)option).ToString(),
                    Selected = option == initial
                });
            }

            return result;
        }

        public static IList<SelectListItem> GetManufacturers(IManufacturerService manufacturerService, ILocalizationService localizationService)
        {
            var result = new List<SelectListItem> {
                new SelectListItem { Text = localizationService.GetResource("Admin.Common.All"), Value = "0" }
            };

            foreach (var manufacturer in manufacturerService.GetAllManufacturers(showHidden: true)) {
                result.Add(new SelectListItem {
                    Text = manufacturer.Name,
                    Value = manufacturer.Id.ToString()
                });
            }

            return result;
        }

        public static IList<SelectListItem> GetVendors(IVendorService vendorService, ILocalizationService localizationService)
        {
            var result = new List<SelectListItem> {
                new SelectListItem { Text = localizationService.GetResource("Admin.Common.All"), Value = "0" }
            };

            foreach (var vendor in vendorService.GetAllVendors(showHidden: true)) {
                result.Add(new SelectListItem {
                    Text = vendor.Name,
                    Value = vendor.Id.ToString()
                });
            }

            return result;
        }

        public static IList<SelectListItem> GetCategories(ICategoryService categoryService, ILocalizationService localizationService)
        {
            var result = new List<SelectListItem> {
                new SelectListItem { Text = localizationService.GetResource("Admin.Common.All"), Value = "0" }
            };

            foreach (var category in categoryService.GetAllCategories(showHidden: true)) {
                result.Add(new SelectListItem {
                    Text = category.Name,
                    Value = category.Id.ToString()
                });
            }

            return result;
        }
    }
}
