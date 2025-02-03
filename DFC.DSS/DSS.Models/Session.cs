using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Session
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? SessionId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? InteractionId { get; set; }

        public DateTime? DateandTimeOfSession { get; set; }

        public string VenuePostCode { get; set; }

        public bool? SessionAttended { get; set; }

        public ReasonForNonAttendance? ReasonForNonAttendance { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string SubcontractorId { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public string CreatedBy { get; set; }
    }
}
