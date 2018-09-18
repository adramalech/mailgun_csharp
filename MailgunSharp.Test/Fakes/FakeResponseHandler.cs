using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MailgunSharp.Test.Fakes
{
  public class FakeResponseHandler : DelegatingHandler
  {
    private readonly Dictionary<Uri, HttpResponseMessage> responses;

    public FakeResponseHandler()
    {
      this.responses = new Dictionary<Uri, HttpResponseMessage>();
    }

    public void AddResponse(Uri uri, HttpResponseMessage message)
    {
      this.responses.Add(uri, message);
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
      return this.responses.ContainsKey(request.RequestUri) ? this.responses[request.RequestUri] : new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
    }
  }
}