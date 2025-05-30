using Microsoft.AspNetCore.Http;

namespace DSS.Interfaces
{
    public interface IHttpRequestService
    {
        public Guid GetCorrelationId(HttpRequest request);
        public string GetTouchpointId(HttpRequest request);
        public string GetApimUrl(HttpRequest request);
        public string GetSubcontractorId(HttpRequest request);
        public Task<T> GetResourceFromRequest<T>(HttpRequest request);
    }
}
