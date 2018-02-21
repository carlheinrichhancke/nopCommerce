using Nop.Core;
using Nop.Plugin.Shipping.Fastway.Domain;
using Nop.Plugin.Shipping.Fastway.Infrastructure.Api;
using Nop.Plugin.Shipping.Fastway.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Shipping.Fastway.Controllers
{
    [AdminAuthorize]
    public class ShippingFastwayController : BasePluginController
    {
        private readonly FastwaySettings _fastwaySettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        private readonly IFastwayApi _fastwayApi;

        public ShippingFastwayController(FastwaySettings fastwaySettings,
            ISettingService settingService,
            ILocalizationService localizationService,
            IFastwayApi fastwayApi)
        {
            this._fastwaySettings = fastwaySettings;
            this._settingService = settingService;
            this._localizationService = localizationService;

            this._fastwayApi = fastwayApi;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var regionalFranchises = _fastwayApi.GetRegionalFranchisesAsync(_fastwaySettings.Country);

            var model = new FastwayShippingModel {
                RegionalFranchise = _fastwaySettings.RegionalFranchise,
                ApiBaseUrl = _fastwaySettings.ApiBaseUrl,
                ApiKey = _fastwaySettings.ApiKey,
                AdditionalHandlingCharge = _fastwaySettings.AdditionalHandlingCharge,
                EnableVerboseLogging = _fastwaySettings.EnableVerboseLogging
            };

            foreach (FastwayCountries country in Enum.GetValues(typeof(FastwayCountries))) {
                model.AvailableCountries.Add(new SelectListItem {
                    Text = CommonHelper.ConvertEnum(country.ToString()),
                    Value = country.ToString(),
                    Selected = country == _fastwaySettings.Country
                });
            }

            foreach (var regionalFranchise in regionalFranchises.Result.Result) {
                model.AvailableRegionalFranchises.Add(new SelectListItem {
                    Text = regionalFranchise.FranchiseName,
                    Value = regionalFranchise.FranchiseCode,
                    Selected = regionalFranchise.FranchiseCode == _fastwaySettings.RegionalFranchise
                });
            }

            return View("~/Plugins/Shipping.Fastway/Views/ShippingFastway/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Configure(FastwayShippingModel model)
        {
            if (!ModelState.IsValid) {
                return Configure();
            }

            // save settings
            _fastwaySettings.Country = (FastwayCountries)Enum.Parse(typeof(FastwayCountries), model.Country);
            _fastwaySettings.RegionalFranchise = model.RegionalFranchise;
            _fastwaySettings.ApiBaseUrl = model.ApiBaseUrl;
            _fastwaySettings.ApiKey = model.ApiKey;
            _fastwaySettings.AdditionalHandlingCharge = model.AdditionalHandlingCharge;
            _fastwaySettings.EnableVerboseLogging = model.EnableVerboseLogging;
            _settingService.SaveSetting(_fastwaySettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public ActionResult RegionalFranchises(FastwayCountries country)
        {
            var rf = _fastwayApi.GetRegionalFranchisesAsync(country).Result;

            return Json(rf.Result.Select(e => new SelectListItem {
                Text = e.FranchiseName,
                Value = e.FranchiseCode
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
