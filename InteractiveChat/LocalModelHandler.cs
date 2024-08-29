using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveChat
{
  
    public class LocalModelHandler : HttpClientHandler
    {
        public LocalModelHandler(string customUrl)
        {
            CustomUrl = customUrl;
        }
        string CustomUrl { get; set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            request.RequestUri=new Uri($"{CustomUrl}{request.RequestUri.PathAndQuery}");
            Task<HttpResponseMessage> task = base.SendAsync(request, cancellationToken);
            return task;
        }
    }
}
