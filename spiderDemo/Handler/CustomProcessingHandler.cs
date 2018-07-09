using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace spiderDemo.Handler
{
    public class CustomProcessingHandler : MessageProcessingHandler
    {
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Post)
            {
                request.Headers.TryAddWithoutValidation("RequestMethod", request.Method.Method);
                request.Method = HttpMethod.Post;
            }
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var request = response.RequestMessage;
            if (request.Headers.Contains("RequestMethod"))
            {
                IEnumerable<string> values;

                if (request.Headers.TryGetValues("RequestMethod", out values))
                {

                    request.Method = new HttpMethod(values.First());
                }
            }
            return response;
        }
    }
}
