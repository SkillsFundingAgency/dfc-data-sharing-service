using System;

namespace DSS.Swagger.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
}