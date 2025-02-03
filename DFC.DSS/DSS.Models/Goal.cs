using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Goal
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? GoalId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? ActionPlanId { get; set; }

        public string SubcontractorId { get; set; }

        public DateTime? DateGoalCaptured { get; set; }

        public DateTime? DateGoalShouldBeCompletedBy { get; set; }

        public DateTime? DateGoalAchieved { get; set; }

        public string GoalSummary { get; set; }

        public GoalType? GoalType { get; set; }

        public GoalStatus? GoalStatus { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public string CreatedBy { get; set; }
    }
}
