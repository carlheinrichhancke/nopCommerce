using Nop.Plugin.Shipping.Fastway.Infrastructure.Api;
using Nop.Services.Logging;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway
{
    public class FastwayShipmentTracker : IShipmentTracker
    {
        private readonly FastwaySettings _fastwaySettings;
        private readonly ILogger _logger;

        private readonly IFastwayApi _fastwayApi;

        public FastwayShipmentTracker(FastwaySettings fastwaySettings,
            ILogger logger,
            IFastwayApi fastwayApi)
        {
            this._fastwaySettings = fastwaySettings;
            this._logger = logger;

            this._fastwayApi = fastwayApi;
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            var events = new List<ShipmentStatusEvent>();

            try {
                var trackAndTrace = _fastwayApi.GetTrackAndTrace(trackingNumber);

                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Fastway track & trace retrieved for tracking number {trackingNumber}. Total scans: {trackAndTrace.Result.Scans.Count}.");

                events = trackAndTrace.Result.Scans.Select(e => new ShipmentStatusEvent {
                    EventName = e.StatusDescription,
                    Location = e.Name,
                    CountryCode = e.Franchise,
                    Date = e.Date
                }).ToList();

                if (_fastwaySettings.EnableVerboseLogging)
                    _logger.Information($"Fastway track & trace successfully transformed to nopCommerce shipment events for tracking number {trackingNumber}");
            } catch (Exception ex) {
                _logger.Error($"Error occurred while retrieving Fastway shipment events for tracking number {trackingNumber}. Exception message: {ex.Message}", ex);
            }

            return events;
        }

        /// <summary>
        /// Gets a url for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>A url to a tracking page.</returns>
        public string GetUrl(string trackingNumber)
        {
            return "http://www.fastway.co.za/our-services/track-your-parcel";
        }

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public bool IsMatch(string trackingNumber)
        {
            if (String.IsNullOrWhiteSpace(trackingNumber))
                return false;

            return trackingNumber.StartsWith("3a");
        }
    }
}
