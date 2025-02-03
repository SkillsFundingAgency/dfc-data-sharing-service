using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class DiversityDetail
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? DiversityId { get; set; }

        public Guid? CustomerId { get; set; }

        public bool? ConsentToCollectLLDDHealth { get; set; }

        public LearningDifficultyOrDisabilityDeclaration? LearningDifficultyOrDisabilityDeclaration { get; set; }

        public PrimaryLearningDifficultyOrDisability? PrimaryLearningDifficultyOrDisability { get; set; }

        public SecondaryLearningDifficultyOrDisability? SecondaryLearningDifficultyOrDisability { get; set; }

        public DateTime? DateAndTimeLLDDHealthConsentCollected { get; set; }

        public bool? ConsentToCollectEthnicity { get; set; }

        public Ethnicity? Ethnicity { get; set; }

        public DateTime? DateAndTimeEthnicityCollected { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public string CreatedBy { get; set; }
    }
}
