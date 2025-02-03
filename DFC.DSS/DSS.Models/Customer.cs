using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Customer
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? CustomerId { get; set; } = Guid.NewGuid();

        public DateTime? DateOfRegistration { get; set; }

        public Title? Title { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public DateTime? DateofBirth { get; set; }

        public Gender? Gender { get; set; }

        public string UniqueLearnerNumber { get; set; }

        public bool? OptInUserResearch { get; set; }

        public bool? OptInMarketResearch { get; set; }

        public DateTime? DateOfTermination { get; set; }

        public ReasonForTermination? ReasonForTermination { get; set; }

        public IntroducedBy? IntroducedBy { get; set; }

        public string IntroducedByAdditionalInfo { get; set; }

        public string SubcontractorId { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public List<PriorityCustomer> PriorityGroups { get; set; }

        public string CreatedBy { get; set; }
    }
}
