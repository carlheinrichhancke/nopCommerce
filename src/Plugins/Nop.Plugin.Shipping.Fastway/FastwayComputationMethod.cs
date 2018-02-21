using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.Fastway.Domain;
using Nop.Plugin.Shipping.Fastway.Infrastructure.Api;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Shipping.Fastway
{
    public class FastwayComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        private readonly FastwaySettings _fastwaySettings;
        private readonly ISettingService _settingService;
        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ILogger _logger;

        private readonly IFastwayApi _fastwayApi;

        public FastwayComputationMethod(FastwaySettings fastwaySettings,
            ISettingService settingService,
            IMeasureService measureService,
            IShippingService shippingService,
            ILogger logger,
            IFastwayApi fastwayApi)
        {
            this._fastwaySettings = fastwaySettings;
            this._settingService = settingService;
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._logger = logger;

            this._fastwayApi = fastwayApi;
        }

        #region Utilities
        private MeasureWeight TargetWeightMeasurement
        {
            get { return _measureService.GetMeasureWeightBySystemKeyword("kg"); }
        }

        private MeasureDimension TargetDimensionMeasurement
        {
            get { return _measureService.GetMeasureDimensionBySystemKeyword("millimetres"); }
        }
        #endregion

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get {
                return ShippingRateComputationMethodType.Realtime;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get {
                return new FastwayShipmentTracker(_fastwaySettings, _logger, _fastwayApi);
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ShippingFastway";
            routeValues = new RouteValueDictionary {
                { "Namespaces", "Nop.Plugin.Shipping.Fastway.Controllers" },
                { "area", null }
            };
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        /// <summary>
        /// Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
                response.AddError("No shipment items");

            if (getShippingOptionRequest.ShippingAddress == null)
                response.AddError("No shipping address");

            if (String.IsNullOrWhiteSpace(getShippingOptionRequest.ShippingAddress.City))
                response.AddError("No shipping address city");

            if (String.IsNullOrWhiteSpace(getShippingOptionRequest.ShippingAddress.ZipPostalCode))
                response.AddError("No shipping address postal code");

            // we've first run all the validation checks & then return any occurances so customers
            // know what all needs fixing in one response
            if (response.Errors.Count > 0)
                return response;
            
            _shippingService.GetDimensions(getShippingOptionRequest.Items, out decimal widthTmp, out decimal lengthTmp, out decimal heightTmp);
            var weightTmp = _shippingService.GetTotalWeight(getShippingOptionRequest);

            // there's no built-in measurement for centimeters, so convert the dims from millimeters
            var length = Convert.ToSingle(_measureService.ConvertFromPrimaryMeasureDimension(lengthTmp, TargetDimensionMeasurement) * 0.1m);
            var width = Convert.ToSingle(_measureService.ConvertFromPrimaryMeasureDimension(widthTmp, TargetDimensionMeasurement) * 0.1m);
            var height = Convert.ToSingle(_measureService.ConvertFromPrimaryMeasureDimension(heightTmp, TargetDimensionMeasurement) * 0.1m);
            var weight = Convert.ToSingle(_measureService.ConvertFromPrimaryMeasureWeight(weightTmp, TargetWeightMeasurement));

            try {
                var rates = _fastwayApi.GetShippingRates(
                    _fastwaySettings.RegionalFranchise,
                    getShippingOptionRequest.ShippingAddress.City,
                    getShippingOptionRequest.ShippingAddress.ZipPostalCode,
                    weight,
                    length,
                    width,
                    height);

                foreach (var psc in rates.Result.Services) {
                    response.ShippingOptions.Add(new Core.Domain.Shipping.ShippingOption {
                        Name = $"Fastway {psc.Name}",
                        Rate = Convert.ToDecimal(psc.TotalPriceNormal) + _fastwaySettings.AdditionalHandlingCharge
                    });
                }
            } catch (NopException ex) {
                response.AddError(ex.Message);

                _logger.Error($"Nop Exception while retrieving Fastway shipping options. Message: {ex.Message} Stack Trace: {ex.StackTrace}", ex);

                return response;
            } catch (Exception ex) {
                response.AddError("Fastway API Service is currently unavailable, please try again later");

                _logger.Error($"Exception while retrieving Fastway shipping options. Message: {ex.Message} Stack Trace: {ex.StackTrace}", ex);

                return response;
            }

            return response;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            var settings = new FastwaySettings {
                Country = FastwayCountries.SouthAfrica,
                ApiBaseUrl = @"https://api.fastway.org/v2"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.Country.Hint", "Specify the country in which the Fastway service is to be used");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.RegionalFranchise", "Regional Franchise");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.RegionalFranchise.Hint", "Specify the regional franchise to dispatch shipments from");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ValidApiKeyMessage", "NOTE: Regional Franchise setting will only become available once a valid API Key is saved");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiBaseUrl", "API Base URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiBaseUrl.Hint", "Specify the base URL to be used for Fastway API calls");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiKey", "API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiKey.Hint", "Specify the Fastway API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.AdditionalHandlingCharge", "Additional Handling Charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.AdditionalHandlingCharge.Hint", "Specify the additional handling fee to charge your customers");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.EnableVerboseLogging", "Enable Verbose Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fastway.Fields.EnableVerboseLogging.Hint", "Toggles verbose logging which, if enabled, writes additional log entries for tracing purposes");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<FastwaySettings>();

            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.RegionalFranchise");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.RegionalFranchise.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ValidApiKeyMessage");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiBaseUrl");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiBaseUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiKey");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.ApiKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.AdditionalHandlingCharge");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.AdditionalHandlingCharge.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.EnableVerboseLogging");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fastway.Fields.EnableVerboseLogging.Hint");

            base.Uninstall();
        }
    }
}
