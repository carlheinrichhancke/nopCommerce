using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway.Domain
{
    public class FastwayDeliverySuburbs
    {
        public FastwayDeliverySuburbs()
        {
            Result = new List<FastwayDeliverySuburbResult>();
        }

        [JsonProperty(PropertyName = "result")]
        public List<FastwayDeliverySuburbResult> Result { get; set; }

        [JsonProperty(PropertyName = "generated_in")]
        public string GeneratedIn { get; set; }
    }

    public class FastwayDeliverySuburbResult
    {
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
    }
}
