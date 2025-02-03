using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Outcome
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? OutcomeId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? ActionPlanId { get; set; }

        public Guid? SessionId { get; set; }

        public string SubcontractorId { get; set; }

        public OutcomeType? OutcomeType { get; set; }

        public DateTime? OutcomeClaimedDate { get; set; }

        public DateTime? OutcomeEffectiveDate { get; set; }
        
        public bool? IsPriorityCustomer { get; set; }

        public string TouchpointId { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string CreatedBy { get; set; }
    }
}
