using DSS.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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

        public string GetApimUrl(HttpRequest request)
        {
            var apimUrl = request.Headers["apimurl"].FirstOrDefault();

            if (!string.IsNullOrEmpty(apimUrl) && apimUrl.EndsWith("/"))
                apimUrl = apimUrl.Substring(0, apimUrl.Length - 1);

            return apimUrl ?? "";
        }

        public string GetSubcontractorId(HttpRequest request)
        {
            return request.Headers["SubcontractorId"].FirstOrDefault() ?? "";
        }

        public async Task<T> GetResourceFromRequest<T>(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Body == null)
                throw new ArgumentNullException(nameof(request.Body));

            request.ContentType = "application/json";

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();

#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<T>(requestBody);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
