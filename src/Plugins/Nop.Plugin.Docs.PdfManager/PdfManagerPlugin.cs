using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Docs.PdfManager
{
    public class PdfManagerPlugin : BasePlugin, IMiscPlugin
    {
        private readonly ISettingService _settingService;

        public PdfManagerPlugin(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "DocsPdfManager";
            routeValues = new RouteValueDictionary {
                { "Namespaces", "Nop.Plugin.Docs.PdfManager.Controllers" },
                { "area", null }
            };
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            var settings = new PdfManagerSettings {
                LogoWidth = 65,
                LogoHeight = 65
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoWidth", "Logo Width");
            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoWidth.Hint", "The maximum width of the logo on PDF documents");
            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoHeight", "Logo Height");
            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoHeight.Hint", "The maximum height of the logo on PDF documents");
            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.EnableVerboseLogging", "Enable Verbose Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Docs.PdfManager.Fields.EnableVerboseLogging.Hint", "Toggles verbose logging which, if enabled, writes additional log entries for tracing purposes");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<PdfManagerSettings>();

            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoWidth");
            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoWidth.Hint");
            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoHeight");
            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.LogoHeight.Hint");
            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.EnableVerboseLogging");
            this.DeletePluginLocaleResource("Plugins.Docs.PdfManager.Fields.EnableVerboseLogging.Hint");

            base.Uninstall();
        }
    }
}
