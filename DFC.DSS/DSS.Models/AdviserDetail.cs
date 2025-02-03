using Newtonsoft.Json;

namespace DSS.Models
{
    public class AdviserDetail
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? AdviserDetailId { get; set; }

        public string AdviserName { get; set; }

        public string AdviserEmailAddress { get; set; }

        public string AdviserContactNumber { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string SubcontractorId { get; set; }

        public string CreatedBy { get; set; }
    }
}
