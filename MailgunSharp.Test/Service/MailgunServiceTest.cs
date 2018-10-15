using System;
using System.Net.Http;
using System.Security.Cryptography;
using MailgunSharp;
using MailgunSharp.Test.Fakes;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace MailgunSharp.Test.Service
{
  public class MailgunServiceTest
  {
    private readonly string apikey;
    private readonly string companyDomain;
    private Mock<FakeHttpResponseHandler> fakeHttpResponseHandler;
    private HttpClient httpClient;
    private IMailgunService service;


    public MailgunServiceTest()
    {
      this.apikey = "apikeyhere";
      this.companyDomain = @"https://companydomainnamehere.com";
      this.fakeHttpResponseHandler = new Mock<FakeHttpResponseHandler>() { CallBase = true };
      this.httpClient = new HttpClient(this.fakeHttpResponseHandler.Object);
      this.service = new MailgunService(this.companyDomain, this.apikey, this.httpClient);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Service_Empty_CompanyDomainName_Should_Throw_Exception(string companyDomainName)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var service = new MailgunService(companyDomainName, this.apikey, this.httpClient);
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
        var service = new MailgunService(this.companyDomain, apikey, this.httpClient);
      });
    }

    [Theory]
    [InlineData("example")]
    [InlineData("example.c")]
    public void Service_Incorrect_Formatt_CompanyDomainName_Should_Throw_Exception(string companyDomainName)
    {
      Assert.Throws<FormatException>(() =>
      {
        var service = new MailgunService(companyDomainName, this.apikey, this.httpClient);
      });
    }


  }
}