using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Products.BulkPriceUpdate
{
    public class BulkPriceUpdatePlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public BulkPriceUpdatePlugin(ISettingService settingService,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ProductsBulkPriceUpdate";
            routeValues = new RouteValueDictionary {
                { "Namespaces", "Nop.Plugin.Products.BulkPriceUpdate.Controllers" },
                { "area", null }
            };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode {
                SystemName = "ProductsBulkPriceUpdate",
                Title = _localizationService.GetResource("Plugins.Products.BulkPriceUpdate"),
                ControllerName = "ProductsBulkPriceUpdate",
                ActionName = "BulkPriceUpdate",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary { { "area", null } },
            };

            var catalogNode = rootNode.ChildNodes.FirstOrDefault(e => e.SystemName == "Catalog");
            if (catalogNode != null) {
                var bulkEditNode = catalogNode.ChildNodes.FirstOrDefault(e => e.SystemName == "Bulk edit products");
                if (bulkEditNode != null) {
                    var idx = catalogNode.ChildNodes.IndexOf(bulkEditNode);
                    catalogNode.ChildNodes.Insert(idx + 1, menuItem);
                } else {
                    catalogNode.ChildNodes.Add(menuItem);
                }
            } else {
                rootNode.ChildNodes.Add(menuItem);
            }
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            var settings = new BulkPriceUpdateSettings();
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate", "Bulk price update");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateBy", "Update by");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateBy.Hint", "The grouping of products to be updated");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Cost", "Cost");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Cost.Hint", "Enable to update product costs");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Price", "Price");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Price.Hint", "Enable to update prices");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Percentage", "Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.FlatAmount", "Flat Amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateOldPrice", "Update Old Price");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateOldPrice.Hint", "Enable to update the Old Price for products with the previous Price");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Saved", "Prices updated successfully");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Fields.EnableVerboseLogging", "Enable Verbose Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Fields.EnableVerboseLogging.Hint", "Toggles verbose logging which, if enabled, writes additional log entries for tracing purposes");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<BulkPriceUpdateSettings>();

            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateBy");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateBy.Hint");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Cost");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Cost.Hint");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Price");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Price.Hint");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Percentage");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.FlatAmount");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateOldPrice");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.UpdateOldPrice.Hint");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Saved");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Fields.EnableVerboseLogging");
            this.DeletePluginLocaleResource("Plugins.Products.BulkPriceUpdate.Fields.EnableVerboseLogging.Hint");

            base.Uninstall();
        }
    }
}
