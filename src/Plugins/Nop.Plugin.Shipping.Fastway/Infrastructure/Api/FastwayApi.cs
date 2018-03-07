using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Plugin.Shipping.Fastway.Domain;
using Nop.Services.Logging;

namespace Nop.Plugin.Shipping.Fastway.Infrastructure.Api
{
    public class FastwayApi : IFastwayApi
    {
        private readonly FastwaySettings _fastwaySettings;
        private readonly ILogger _logger;

        public FastwayApi(FastwaySettings fastwaySettings,
            ILogger logger)
        {
            this._fastwaySettings = fastwaySettings;
            this._logger = logger;
        }

        /// <summary>
        /// Get available regional franchises for a country asynchronously
        /// </summary>
        /// <param name="country">The country for which to retrieve the regional franchises</param>
        /// <returns>Fastway Regional Franchises for country</returns>
        public async Task<FastwayRegionalFranchises> GetRegionalFranchisesAsync(FastwayCountries country)
        {
            var regionalFranchises = new FastwayRegionalFranchises();

            if (String.IsNullOrWhiteSpace(_fastwaySettings.ApiKey))
                _logger.Warning($"No Fastway API Key set. Could not retrieve regional franchises for country {country}");

            try {
                var apiUrl = $"{_fastwaySettings.ApiBaseUrl}/psc/listrfs?CountryCode={(int)country}&api_key={_fastwaySettings.ApiKey}";
                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Retrieving Fastway regional franchises using API URL: {apiUrl}");

                using (var httpClient = new HttpClient()) {
                    var resp = httpClient.GetAsync(apiUrl).Result;
                    var json = await resp.Content.ReadAsStringAsync();

                    if (_fastwaySettings.EnableVerboseLogging)
                        _logger.Information($"Fastway regional franchises successfully retrieved for country {country}");

                    regionalFranchises = JsonConvert.DeserializeObject<FastwayRegionalFranchises>(json);
                }
            } catch (Exception ex) {
                _logger.Error($"Error occurred while retrieving Fastway regional franchises. Exception message: {ex.Message}", ex);
            }

            return regionalFranchises;
        }

        /// <summary>
        /// Get available delivery suburbs for a Regional Franchise & by an optional Search Term
        /// </summary>
        /// <param name="rfcode">The Regional Franchise Code of the shipment pickup franchise</param>
        /// <param name="searchTerm">The Search Term to additionally apply to the results</param>
        /// <returns>Fastway Delivery Suburbs</returns>
        public async Task<FastwayDeliverySuburbs> GetDeliverySuburbsAsync(string rfcode, string searchTerm = null)
        {
            var deliverySuburbs = new FastwayDeliverySuburbs();

            if (String.IsNullOrWhiteSpace(_fastwaySettings.ApiKey))
                _logger.Warning($"No Fastway API Key set. Could not retrieve delivery suburbs for regional franchise {rfcode} & search term {searchTerm}");

            try {
                var apiUrl = $"{_fastwaySettings.ApiBaseUrl}/psc/listdeliverysuburbs/{rfcode}/{searchTerm}?api_key={_fastwaySettings.ApiKey}";
                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Retrieving Fastway delivery suburbs using API URL: {apiUrl}");

                using (var httpClient = new HttpClient()) {
                    var resp = httpClient.GetAsync(apiUrl).Result;
                    var json = await resp.Content.ReadAsStringAsync();

                    if (_fastwaySettings.EnableVerboseLogging)
                        _logger.Information($"Fastway delivery suburbs successfully retrieved for regional franchise {rfcode} & search term {searchTerm}");

                    deliverySuburbs = JsonConvert.DeserializeObject<FastwayDeliverySuburbs>(json);
                }
            } catch (Exception ex) {
                _logger.Error($"Error occurred while retrieving Fastway delivery suburbs. Exception message: {ex.Message}", ex);
            }

            return deliverySuburbs;
        }

        /// <summary>
        /// Get shipping services for a shipment
        /// </summary>
        /// <param name="rfcode">The Regional Franchise Code of the shipment pickup franchise</param>
        /// <param name="destCity">The destination city of the shipment</param>
        /// <param name="destPostCode">The destination post code of the shipment</param>
        /// <param name="weightKg">The shipment weight in kilograms</param>
        /// <param name="lengthCm">The length of the shipment in centimeters</param>
        /// <param name="widthCm">The width of the shipment in centimeters</param>
        /// <param name="heightCm">The height of the shipment in centimeters</param>
        /// <returns>Shipping services</returns>
        public FastwayPriceServiceCalculator GetShippingRates(string rfcode,
            string destCity,
            string destPostCode,
            float weightKg,
            float lengthCm,
            float widthCm,
            float heightCm)
        {
            var shippingRates = new FastwayPriceServiceCalculator();

            if (String.IsNullOrWhiteSpace(_fastwaySettings.ApiKey))
                _logger.Warning($"No Fastway API Key set. Could not retrieve shipping rates");

            try {
                var apiUrl = $"{_fastwaySettings.ApiBaseUrl}/psc/lookup/{rfcode}/{destCity}/{destPostCode}/{weightKg}?api_key={_fastwaySettings.ApiKey}&LengthInCm={lengthCm}&WidthInCm={widthCm}&HeightInCm={heightCm}";
                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Retrieving Fastway shipping rates using API URL: {apiUrl}");

                using (var httpClient = new HttpClient()) {
                    var resp = httpClient.GetAsync(apiUrl).Result;
                    var json = resp.Content.ReadAsStringAsync().Result;

                    if (_fastwaySettings.EnableVerboseLogging)
                        _logger.Information($"Fastway shipping rates successfully retrieved to {destCity}, {destPostCode}");

                    shippingRates = JsonConvert.DeserializeObject<FastwayPriceServiceCalculator>(json);
                }
            } catch (Exception ex) {
                _logger.Error($"Error occurred while retrieving Fastway shipping rates. Exception message: {ex.Message}", ex);
            }

            return shippingRates;
        }

        /// <summary>
        /// Gets the Track and Trace data for a given tracking number
        /// </summary>
        /// <param name="trackingNumber">The tracking number to query</param>
        /// <returns>Fastway Track and Trace details</returns>
        public FastwayTrackAndTrace GetTrackAndTrace(string trackingNumber)
        {
            var trackAndTrace = new FastwayTrackAndTrace();

            if (String.IsNullOrWhiteSpace(_fastwaySettings.ApiKey))
                _logger.Warning($"No Fastway API Key set. Could not retrieve track & trace for tracking number {trackingNumber}");

            try {
                var apiUrl = $"{_fastwaySettings.ApiBaseUrl}/tracktrace/detail/{trackingNumber}?api_key={_fastwaySettings.ApiKey}";
                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Retrieving Fastway track & trace using API URL: {apiUrl}");

                using (var httpClient = new HttpClient()) {
                    var resp = httpClient.GetAsync(apiUrl).Result;
                    var json = resp.Content.ReadAsStringAsync().Result;

                    if (_fastwaySettings.EnableVerboseLogging)
                        _logger.Information($"Fastway track & trace request successfully completed for tracking number {trackingNumber}");

                    trackAndTrace = JsonConvert.DeserializeObject<FastwayTrackAndTrace>(json);
                }
            } catch (Exception ex) {
                _logger.Error($"Error occurred while retrieving Fastway track & trace for tracking number {trackingNumber}. Exception message: {ex.Message}", ex);
            }

            return trackAndTrace;
        }
    }
}
