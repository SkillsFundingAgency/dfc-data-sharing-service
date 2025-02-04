using DSS.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DSS.SharedServices
{
    public class HttpRequestService : IHttpRequestService
    {
        public Guid GetCorrelationId(HttpRequest request)
        {
            string reqCorrelationId = request.Headers["DssCorrelationId"].FirstOrDefault() ?? "";

            if (string.IsNullOrWhiteSpace(reqCorrelationId) || !Guid.TryParse(reqCorrelationId, out var reqCorrelationGuid))
            {
                return Guid.NewGuid();
            }

            return reqCorrelationGuid;
        }

        public string GetTouchpointId(HttpRequest request)
        {
            return request.Headers["TouchpointId"].FirstOrDefault() ?? "";
        }
    }
}
