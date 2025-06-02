namespace DSS.Swagger.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Example : Attribute
    {
        public string? Description { get; set; }
    }
}