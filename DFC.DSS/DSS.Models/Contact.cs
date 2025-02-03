using DSS.Models.Enums;
using Newtonsoft.Json;

namespace DSS.Models
{
    public class Contact
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? ContactId { get; set; }

        public Guid? CustomerId { get; set; }

        public PreferredContactMethod? PreferredContactMethod { get; set; }

        public string MobileNumber { get; set; }

        public string HomeNumber { get; set; }

        public string AlternativeNumber { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }

        public bool? IsDigitalAccount { get; set; }
        
        public string FirstName { get; private set; }
        
        public string LastName { get; private set; }

        public bool? ChangeEmailAddress { get; private set; }
        
        public string CurrentEmail { get; private set; }
        
        public string NewEmail { get; private set; }
        
        public Guid? IdentityStoreId { get; private set; }
    }
}
