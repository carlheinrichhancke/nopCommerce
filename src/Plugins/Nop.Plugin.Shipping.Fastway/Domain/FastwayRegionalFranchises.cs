using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway.Domain
{
    public class FastwayRegionalFranchises
    {
        public FastwayRegionalFranchises()
        {
            Result = new List<FastwayRegionalFranchiseResult>();
        }

        [JsonProperty(PropertyName = "result")]
        public List<FastwayRegionalFranchiseResult> Result { get; set; }
    }

    public class FastwayRegionalFranchiseResult
    {
        public string FranchiseCode { get; set; }
        public string FranchiseName { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "fax")]
        public string Fax { get; set; }

        [JsonProperty(PropertyName = "Add1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "Add2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "Add3")]
        public string Address3 { get; set; }

        [JsonProperty(PropertyName = "Add4")]
        public string Address4 { get; set; }
    }
}
