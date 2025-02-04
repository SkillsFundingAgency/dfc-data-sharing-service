using Microsoft.AspNetCore.Http;

namespace DSS.Interfaces
{
    public interface IHttpRequestService
    {
        public Guid GetCorrelationId(HttpRequest request);
        public string GetTouchpointId(HttpRequest request);
    }
}
