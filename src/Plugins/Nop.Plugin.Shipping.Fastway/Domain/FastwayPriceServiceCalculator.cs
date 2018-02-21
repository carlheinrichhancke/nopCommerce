using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway.Domain
{
    public class FastwayPriceServiceCalculator
    {
        public FastwayPriceServiceCalculator()
        {
            Result = new FastwayPSCResult();
        }

        [JsonProperty(PropertyName = "generated_in")]
        public string GeneratedIn { get; set; }

        [JsonProperty(PropertyName = "result")]
        public FastwayPSCResult Result { get; set; }
    }

    public class FastwayPSCResult
    {
        public FastwayPSCResult()
        {
            Services = new List<FastwayPSCService>();
        }

        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string PostCode { get; set; }

        [JsonProperty(PropertyName = "delfranchise")]
        public string DeliveryFranchise { get; set; }

        [JsonProperty(PropertyName = "delfranchise_code")]
        public string DeliveryFranchiseCode { get; set; }

        [JsonProperty(PropertyName = "pickfranchise")]
        public string PickFranchise { get; set; }

        [JsonProperty(PropertyName = "pickfranchise_code")]
        public string PickFranchiseCode { get; set; }

        [JsonProperty(PropertyName = "delivery_timeframe_days")]
        public string DeliveryTimeframeDays { get; set; }

        [JsonProperty(PropertyName = "parcel_weight_kg")]
        public float ParcelWeightKg { get; set; }

        [JsonProperty(PropertyName = "services")]
        public List<FastwayPSCService> Services { get; set; }
    }

    public class FastwayPSCService
    {
        public FastwayPSCService()
        {
            LabelColours = new List<string>();
            LabelColoursPretty = new List<string>();
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "labelcolour")]
        public string LabelColour { get; set; }

        [JsonProperty(PropertyName = "labelcolour_array")]
        public List<string> LabelColours { get; set; }

        [JsonProperty(PropertyName = "labelcolour_pretty")]
        public string LabelColourPretty { get; set; }

        [JsonProperty(PropertyName = "labelcolour_pretty_array")]
        public List<string> LabelColoursPretty { get; set; }

        [JsonProperty(PropertyName = "weightlimit")]
        public float WeightLimit { get; set; }

        [JsonProperty(PropertyName = "excess_labels_required")]
        public int ExcessLabelsRequired { get; set; }

        [JsonProperty(PropertyName = "excess_label_price_normal")]
        public string ExcessLabelPriceNormal { get; set; }

        [JsonProperty(PropertyName = "excess_label_price_frequent")]
        public string ExcessLabelPriceFrequent { get; set; }

        [JsonProperty(PropertyName = "excess_label_price_normal_exgst")]
        public string ExcessLabelPriceNormalExGst { get; set; }

        [JsonProperty(PropertyName = "excess_label_price_frequent_exgst")]
        public string ExcessLabelPriceFrequentExGst { get; set; }

        [JsonProperty(PropertyName = "labelprice_normal")]
        public string LabelPriceNormal { get; set; }

        [JsonProperty(PropertyName = "labelprice_frequent")]
        public string LabelPriceFrequent { get; set; }

        [JsonProperty(PropertyName = "labelprice_normal_exgst")]
        public float LabelPriceNormalExGst { get; set; }

        [JsonProperty(PropertyName = "labelprice_frequent_exgst")]
        public float LabelPriceFrequentExGst { get; set; }

        [JsonProperty(PropertyName = "totalprice_normal")]
        public string TotalPriceNormal { get; set; }

        [JsonProperty(PropertyName = "totalprice_frequent")]
        public string TotalPriceFrequent { get; set; }

        [JsonProperty(PropertyName = "totalprice_normal_exgst")]
        public string TotalPriceNormalExGst { get; set; }

        [JsonProperty(PropertyName = "totalprice_frequent_exgst")]
        public string TotalPriceFrequentExGst { get; set; }

        [JsonProperty(PropertyName = "rural_labels_required")]
        public int RuralLabelsRequired { get; set; }
    }
}
