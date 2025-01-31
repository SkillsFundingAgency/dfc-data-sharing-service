using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class ActionPlan
    {
        [JsonProperty("id")]
        public Guid? ActionPlanId { get; set; } = Guid.NewGuid();

        public Guid? CustomerId { get; set; }

        public Guid? InteractionId { get; set; }

        public Guid? SessionId { get; set; }

        public string SubcontractorId { get; set; }

        public DateTime? DateActionPlanCreated { get; set; }

        public bool? CustomerCharterShownToCustomer { get; set; }

        public DateTime? DateAndTimeCharterShown { get; set; }

        public DateTime? DateActionPlanSentToCustomer { get; set; }

        public ActionPlanDeliveryMethod? ActionPlanDeliveryMethod { get; set; }

        public DateTime? DateActionPlanAcknowledged { get; set; }

        public string CurrentSituation { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string CreatedBy { get; set; }

        public CustomerSatisfaction? CustomerSatisfaction { get; set; }
    }
}
