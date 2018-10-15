using System;
using System.Net;
using System.Net.Http;
using MailgunSharp.Test.Fakes;
using Moq;
using Xunit;

namespace MailgunSharp.Test.Service
{
  public class MailgunServiceTest
  {
    private readonly string apikey;
    private readonly string companyDomain;
    //private Mock<FakeHttpResponseHandler> fakeHttpResponseHandler;
    //private HttpClient httpClient;

    public MailgunServiceTest()
    {
      this.apikey = "apikeyhere";
      this.companyDomain = @"https://companydomainnamehere.com";
      //this.fakeHttpResponseHandler = new Mock<FakeHttpResponseHandler>() { CallBase = true };
      //this.httpClient = new HttpClient(this.fakeHttpResponseHandler.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Service_Empty_CompanyDomainName_Should_Throw_Exception(string companyDomainName)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var service = new MailgunService(companyDomainName, this.apikey);
      });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Service_Empty_ApiKey_Should_Throw_Exception(string apikey)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var service = new MailgunService(this.companyDomain, apikey);
      });
    }

    [Theory]
    [InlineData("example")]
    [InlineData("example.c")]
    public void Service_Incorrect_Format_CompanyDomainName_Should_Throw_Exception(string companyDomainName)
    {
      Assert.Throws<FormatException>(() =>
      {
        var service = new MailgunService(companyDomainName, this.apikey);
      });
    }
  }
}