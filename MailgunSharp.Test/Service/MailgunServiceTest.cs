using System;
using System.Net.Http;
using MailgunSharp;
using MailgunSharp.Test.Fakes;
using Xunit;
using VaultSharp;

namespace MailgunSharp.Test.Service
{
  public class MailgunServiceTest
  {
    private readonly string apikey;
    private readonly string companyDomain;

    public MailgunServiceTest()
    {
      this.apikey = "";
      this.companyDomain = @"";
    }

    [Fact]
    public void TestService()
    {
      var service = new MailgunService(this.companyDomain, this.apikey, new HttpClient(new FakeResponseHandler()));


    }
  }
}