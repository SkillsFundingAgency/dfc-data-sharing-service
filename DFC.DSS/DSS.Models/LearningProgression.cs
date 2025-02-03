using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class LearningProgression
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? LearningProgressionId { get; set; } = Guid.NewGuid();

        public Guid? CustomerId { get; set; }

        public DateTime? DateProgressionRecorded { get; set; }

        public CurrentLearningStatus? CurrentLearningStatus { get; set; }

        public LearningHours? LearningHours { get; set; }

        public DateTime? DateLearningStarted { get; set; }

        public QualificationLevel? CurrentQualificationLevel { get; set; }

        public DateTime? DateQualificationLevelAchieved { get; set; }

        public string? LastLearningProvidersUKPRN { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? LastModifiedTouchpointId { get; set; }

        public string? CreatedBy { get; set; }
    }
}
