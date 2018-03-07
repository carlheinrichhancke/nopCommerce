using Nop.Plugin.Shipping.Fastway.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway.Infrastructure.Api
{
    public interface IFastwayApi
    {
        /// <summary>
        /// Get available regional franchises for a country asynchronously
        /// </summary>
        /// <param name="country">The country for which to retrieve the regional franchises</param>
        /// <returns>Fastway Regional Franchises for country</returns>
        Task<FastwayRegionalFranchises> GetRegionalFranchisesAsync(FastwayCountries country);

        /// <summary>
        /// Get available delivery suburbs for a Regional Franchise & by an optional Search Term
        /// </summary>
        /// <param name="rfcode">The Regional Franchise Code of the shipment pickup franchise</param>
        /// <param name="searchTerm">The Search Term to additionally apply to the results</param>
        /// <returns>Fastway Delivery Suburbs</returns>
        Task<FastwayDeliverySuburbs> GetDeliverySuburbsAsync(string rfcode, string searchTerm = null);

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
        FastwayPriceServiceCalculator GetShippingRates(string rfcode,
            string destCity,
            string destPostCode,
            float weightKg,
            float lengthCm,
            float widthCm,
            float heightCm);

        /// <summary>
        /// Gets the Track and Trace data for a given tracking number
        /// </summary>
        /// <param name="trackingNumber">The tracking number to query</param>
        /// <returns>Fastway Track and Trace details</returns>
        FastwayTrackAndTrace GetTrackAndTrace(string trackingNumber);
    }
}
