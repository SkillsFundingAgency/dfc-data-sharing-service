using Newtonsoft.Json;

namespace DSS.Models
{
    public class Transfer
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? TransferId { get; set; }

        public Guid CustomerId { get; set; }

        public Guid? InteractionId { get; set; }

        public string OriginatingTouchpointId { get; set; }

        public string TargetTouchpointId { get; set; }

        public string Context { get; set; }

        public DateTime? DateandTimeOfTransfer { get; set; }

        public DateTime? DateandTimeofTransferAccepted { get; set; }

        public DateTime? RequestedCallbackTime { get; set; }

        public DateTime? ActualCallbackTime { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }
    }
}
