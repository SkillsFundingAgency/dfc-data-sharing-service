namespace DSS.Models
{
    public class Notification
    {
        public string? id { get; set; } = Guid.NewGuid().ToString();
        public Guid? CustomerId { get; set; }
        public Uri? ResourceURL { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? TouchpointId { get; set; }
        public Guid CollectionId { get; set; }
    }
}