using Nop.Plugin.Docs.PdfManager.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Docs.PdfManager.Controllers
{
    public class DocsPdfManagerController : BasePluginController
    {
        private readonly PdfManagerSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public DocsPdfManagerController(PdfManagerSettings settings,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._settings = settings;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new PdfManagerConfigModel {
                LogoWidth = (int)Math.Round(_settings.LogoWidth, 0),
                LogoHeight = (int)Math.Round(_settings.LogoHeight, 0),
                EnableVerboseLogging = _settings.EnableVerboseLogging
            };

            return View("~/Plugins/Docs.PdfManager/Views/DocsPdfManager/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Configure(PdfManagerConfigModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            // save settings
            _settings.LogoWidth = model.LogoWidth;
            _settings.LogoHeight = model.LogoHeight;
            _settings.EnableVerboseLogging = model.EnableVerboseLogging;
            _settingService.SaveSetting(_settings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
