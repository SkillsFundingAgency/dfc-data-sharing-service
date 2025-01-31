using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Action
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? ActionId { get; set; } = Guid.NewGuid();

        public Guid? CustomerId { get; set; }

        public Guid? ActionPlanId { get; set; }

        public DateTime? DateActionAgreed { get; set; }

        public DateTime? DateActionAimsToBeCompletedBy { get; set; }

        public DateTime? DateActionActuallyCompleted { get; set; }

        public string ActionSummary { get; set; }

        public string SignpostedTo { get; set; }

        public SignpostedToCategory? SignpostedToCategory { get; set; }

        public ActionType? ActionType { get; set; }

        public ActionStatus? ActionStatus { get; set; }

        public PersonResponsible? PersonResponsible { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string CreatedBy { get; set; }
    }
}
