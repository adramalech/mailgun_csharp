using System;
using Xunit;
using MailgunSharp.Domains;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Domains
{
  public class DomainRequestTest
  {
    [Theory]
    [InlineData("", "")]
    [InlineData(null, "password")]
    [InlineData(@"https://example.com", "  ")]
    [InlineData("  ", "password")]
    [InlineData(@"https://www.example.com", null)]
    public void Initialize_DomainRequest_Should_Throw_Exception_For_Required_Params(string domainName, string smtpPassword)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var domainRequest = new DomainRequest(domainName, smtpPassword);
      });
    }

    [Theory]
    [InlineData(@"https://sample.example.com", "password123", SpamAction.TAG, true, true)]
    public void Initialize_DomainRequest_Should_Have_Same_Values_As_When_Created(string domainName, string smtpPassword, SpamAction spamAction, bool wildcard, bool authority)
    {
      var domainRequest = new DomainRequest(domainName, smtpPassword, spamAction, wildcard, authority);

      Assert.Equal(domainName, domainRequest.DomainName);
      Assert.Equal(smtpPassword, domainRequest.SmtpPassword);
      Assert.Equal(spamAction, domainRequest.SpamAction);
      Assert.Equal(wildcard, domainRequest.Wildcard);
      Assert.Equal(authority, domainRequest.ForceDKIMAuthority);
    }

    [Theory]
    [InlineData(@"domainname.com")]
    [InlineData(@"1234")]
    [InlineData(@"abcdefgh")]
    public void Domain_Request_Should_Throw_Exception_When_Hostname_Incorrectly_Formatted(string domainName)
    {
      Assert.Throws<FormatException>(() =>
      {
        var domainRequest = new DomainRequest(domainName, "password");
      });
    }

    [Theory]
    [InlineData(@"https://sample.example.com", "password123", SpamAction.TAG, true, true)]
    public void DomainRequest_Should_Generate_NonEmpty_FormContent(string domainName, string smtpPassword, SpamAction spamAction, bool wildcard, bool authority)
    {
      var domainRequest = new DomainRequest(domainName, smtpPassword, spamAction, wildcard, authority);

      var formContent = domainRequest.ToFormContent();

      Assert.NotEmpty(formContent);
    }

    [Theory]
    [InlineData(@"https://sample.example.com", "password123", SpamAction.TAG, true, true)]
    public void DomainRequest_Should_Generate_NonEmpty_JsonObject(string domainName, string smtpPassword, SpamAction spamAction, bool wildcard, bool authority)
    {
      var domainRequest = new DomainRequest(domainName, smtpPassword, spamAction, wildcard, authority);

      var json = domainRequest.ToJson();

      Assert.NotEmpty(json);
    }
  }
}