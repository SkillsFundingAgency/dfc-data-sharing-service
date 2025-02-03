using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class EmploymentProgression
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? EmploymentProgressionId { get; set; }

        public Guid? CustomerId { get; set; }

        public DateTime? DateProgressionRecorded { get; set; }

        public CurrentEmploymentStatus? CurrentEmploymentStatus { get; set; }

        public EconomicShockStatus? EconomicShockStatus { get; set; }

        public string EconomicShockCode { get; set; }

        public string EmployerName { get; set; }

        public string EmployerAddress { get; set; }

        public string EmployerPostcode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public EmploymentHours? EmploymentHours { get; set; }

        public DateTime? DateOfEmployment { get; set; }

        public DateTime? DateOfLastEmployment { get; set; }

        public LengthOfUnemployment? LengthOfUnemployment { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public string CreatedBy { get; set; }
    }
}
