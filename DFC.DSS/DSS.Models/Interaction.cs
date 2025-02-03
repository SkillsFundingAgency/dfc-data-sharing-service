using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Interaction
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? InteractionId { get; set; } = Guid.NewGuid();

        public Guid? CustomerId { get; set; }

        public string TouchpointId { get; set; }

        public Guid? AdviserDetailsId { get; set; }

        public DateTime? DateandTimeOfInteraction { get; set; }

        public Channel? Channel { get; set; }

        public InteractionType? InteractionType { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }
    }
}
