using Nop.Core.Configuration;
using Nop.Plugin.Shipping.Fastway.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway
{
    public class FastwaySettings : ISettings
    {
        private string _apiBaseUrl;

        /// <summary>
        /// Gets or sets the Fastway country where service is to be used
        /// </summary>
        public FastwayCountries Country { get; set; }

        /// <summary>
        /// Gets or sets the regional franchise to be used to dispatch the shipments
        /// </summary>
        public string RegionalFranchise { get; set; }

        /// <summary>
        /// Gets or sets the base URL for the API, to which all requests will be made.
        /// </summary>
        public string ApiBaseUrl
        {
            get {
                return CanonicalizeApiUrl(_apiBaseUrl);
            }
            set {
                _apiBaseUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the Fastway API Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the addtional handling charge for Fastway deliveries.
        /// </summary>
        public decimal AdditionalHandlingCharge { get; set; }

        /// <summary>
        /// Gets or sets whether verbose logging is enabled for the Fastway plugin.
        /// </summary>
        public bool EnableVerboseLogging { get; set; }

        /// <summary>
        /// Canonicalize the API Base URL, ensuring that no trailing backslash was added that could break future API calls.
        /// </summary>
        /// <param name="url">The base URL to be normalized</param>
        /// <returns>Either the original URL or the canonicalized version</returns>
        private string CanonicalizeApiUrl(string url)
        {
            if (url.EndsWith("/"))
                return url.Remove(url.LastIndexOf("/"), 1);
            else
                return url;
        }
    }
}
