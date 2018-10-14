using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MailgunSharp.Test.Fakes
{
  public class FakeHttpResponseHandler : DelegatingHandler
  {
    private readonly Dictionary<Uri, HttpResponseMessage> responses;

    public FakeHttpResponseHandler()
    {
      this.responses = new Dictionary<Uri, HttpResponseMessage>();
    }

    public void AddResponse(Uri uri, HttpResponseMessage message)
    {
      this.responses.Add(uri, message);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
      return Task.FromResult(this.responses.ContainsKey(request.RequestUri) ? this.responses[request.RequestUri] : new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });
    }
  }
}