using Newtonsoft.Json;

namespace DSS.Models
{
    public class Collection
    {
        [JsonProperty(PropertyName = "id")]
        public Guid CollectionId { get; set; } = Guid.NewGuid();

        public Uri CollectionReports { get; set; }

        public string TouchPointId { get; set; }

        public string Ukprn { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
