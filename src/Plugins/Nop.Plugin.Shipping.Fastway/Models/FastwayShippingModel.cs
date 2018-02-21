using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Shipping.Fastway.Models
{
    public class FastwayShippingModel
    {
        public FastwayShippingModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableRegionalFranchises = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.Country")]
        public string Country { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.RegionalFranchise")]
        public string RegionalFranchise { get; set; }
        public IList<SelectListItem> AvailableRegionalFranchises { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.ApiBaseUrl")]
        public string ApiBaseUrl { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.Fastway.Fields.EnableVerboseLogging")]
        public bool EnableVerboseLogging { get; set; }
        public bool EnableVerboseLogging_OverrideForStore { get; set; }
    }
}
