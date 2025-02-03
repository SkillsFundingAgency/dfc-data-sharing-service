using Newtonsoft.Json;

namespace DSS.Models
{
    public class Subscription
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? SubscriptionId { get; set; }

        public Guid? CustomerId { get; set; }

        public string TouchPointId { get; set; }

        public bool? Subscribe { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }
    }
}
