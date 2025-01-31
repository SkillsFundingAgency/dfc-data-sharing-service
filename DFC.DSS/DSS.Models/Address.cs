using Newtonsoft.Json;

namespace DSS.Models
{
    public class Address
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? AddressId { get; set; }

        public Guid? CustomerId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string Address5 { get; set; }

        public string PostCode { get; set; }

        public string AlternativePostCode { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string SubcontractorId { get; set; }

        public string CreatedBy { get; set; }
    }
}
