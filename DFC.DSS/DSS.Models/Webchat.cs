using Newtonsoft.Json;

namespace DSS.Models
{
    public class WebChat
    {
        [JsonProperty(PropertyName = "id")]
        public Guid? WebChatId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? InteractionId { get; set; }

        public string DigitalReference { get; set; }

        public DateTime? WebChatStartDateandTime { get; set; }

        public DateTime? WebChatEndDateandTime { get; set; }

        public TimeSpan? WebChatDuration { get; set; }

        public string WebChatNarrative { get; set; }

        public bool? SentToCustomer { get; set; }

        public DateTime? DateandTimeSentToCustomers { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedTouchpointId { get; set; }
    }
}
