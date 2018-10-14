using System;
using System.Net.Http;
using MailgunSharp;
using MailgunSharp.Test.Fakes;
using Moq;
using Xunit;

namespace MailgunSharp.Test.Service
{
  public class MailgunServiceTest
  {
    private readonly string apikey;
    private readonly string companyDomain;
    private Mock<FakeHttpResponseHandler> fakeHttpResponseHandler;
    private HttpClient httpClient;

    public MailgunServiceTest()
    {
      this.apikey = "apikeyhere";
      this.companyDomain = @"companydomainnamehere.com";
      this.fakeHttpResponseHandler = new Mock<FakeHttpResponseHandler>() { CallBase = true };
      this.httpClient = new HttpClient(this.fakeHttpResponseHandler.Object);
    }


  }
}