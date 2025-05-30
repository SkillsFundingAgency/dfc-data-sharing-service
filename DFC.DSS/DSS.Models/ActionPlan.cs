using DSS.Models.Interfaces;
using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class ActionPlan : IActionPlan
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

        public void SetDefaultValues()
        {
            if (!LastModifiedDate.HasValue)
                LastModifiedDate = DateTime.UtcNow;

            if (!CustomerCharterShownToCustomer.HasValue)
                CustomerCharterShownToCustomer = false;

            if (!CustomerSatisfaction.HasValue)
                CustomerSatisfaction = null;
        }

        public void SetIds(Guid customerGuid, Guid interactionGuid, string touchpointId, string subcontractorId)
        {
            ActionPlanId = Guid.NewGuid();
            CustomerId = customerGuid;
            InteractionId = interactionGuid;
            LastModifiedTouchpointId = touchpointId;
            SubcontractorId = subcontractorId;
            CreatedBy = touchpointId;
        }
    }
}
