using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Shipping.Fastway.Domain
{
    public class FastwayTrackAndTrace
    {
        public FastwayTrackAndTrace()
        {
            Result = new FastwayTrackAndTraceResult();
        }

        [JsonProperty(PropertyName = "result")]
        public FastwayTrackAndTraceResult Result { get; set; }

        [JsonProperty(PropertyName = "generated_in")]
        public string GeneratedIn { get; set; }
    }

    public class FastwayTrackAndTraceResult
    {
        public FastwayTrackAndTraceResult()
        {
            Scans = new List<FastwayTrackAndTraceScan>();
        }

        public string LabelNumber { get; set; }
        public string Signature { get; set; }
        public string DistributedTo { get; set; }
        public string Reference { get; set; }
        public string OriginalLabelNumber { get; set; }
        public string CallingCard { get; set; }

        [JsonProperty(PropertyName = "IsOnforward")]
        public string IsOnForward { get; set; }
        
        public List<FastwayTrackAndTraceScan> Scans { get; set; }
    }

    public class FastwayTrackAndTraceScan
    {
        public FastwayTrackAndTraceScan()
        {
            CompanyInfo = new FastwayTrackAndTraceCompanyInfo();
        }

        /// <summary>
        /// The type of scan. P = Pickup, D = Delivery, T = Transit
        /// </summary>
        public string Type { get; set; }

        public string Courier { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime Date
        {
            get {
                return DateTime.ParseExact(StrDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }

        [JsonIgnore]
        public DateTime UploadDate
        {
            get {
                return DateTime.ParseExact(StrUploadDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        [JsonProperty(PropertyName = "Date")]
        public string StrDate { get; set; }

        [JsonProperty(PropertyName = "UploadDate")]
        public string StrUploadDate { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }
        public string Franchise { get; set; }
        public string StatusDescription { get; set; }
        public string Signature { get; set; }
        public FastwayTrackAndTraceCompanyInfo CompanyInfo { get; set; }
    }

    public class FastwayTrackAndTraceCompanyInfo
    {
        [JsonProperty(PropertyName = "contactName")]
        public string ContactName { get; set; }

        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "address3")]
        public string Address3 { get; set; }

        [JsonProperty(PropertyName = "address4")]
        public string Address4 { get; set; }

        [JsonProperty(PropertyName = "address5")]
        public string Address5 { get; set; }

        [JsonProperty(PropertyName = "address6")]
        public string Address6 { get; set; }

        [JsonProperty(PropertyName = "address7")]
        public string Address7 { get; set; }

        [JsonProperty(PropertyName = "address8")]
        public string Address8 { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }
    }
}
