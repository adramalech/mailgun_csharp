using System;
using Xunit;
using MailgunSharp.Domains;

namespace MailgunSharp.Test.Domains
{
  public class DomainCredentialRequestTest
  {
    [Theory]
    [InlineData("  ", "password")]
    [InlineData(null, "password")]
    [InlineData("username", "  ")]
    [InlineData("username", null)]
    [InlineData("", "")]
    public void Initialized_DomainCredential_Without_RequiredFields_Should_Throw_Exception(string username, string password)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var domainCredentialRequest = new DomainCredentialRequest(username, password);
      });
    }

    [Theory]
    [InlineData("min")]
    [InlineData("abcdefghijklmnopqrstuvwxyz0123456789")]
    public void DomainCredentialRequest_Password_Required_Length_Should_Throw_Exception_If_Below_Min_Or_Above_Max_Lengths(string password)
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => {
        var domainCredentialRequest = new DomainCredentialRequest("username", password);
      });
    }

    [Theory]
    [InlineData("username", "password")]
    public void Initialized_DomainCredentails_With_Required_Fields_Should_Be_Assigned_And_Exist(string username, string password)
    {
      var domainCredentialRequest = new DomainCredentialRequest(username, password);

      Assert.Equal(username, domainCredentialRequest.Username);
      Assert.Equal(password, domainCredentialRequest.Password);
    }

    [Theory]
    [InlineData("username", "password")]
    public void Initialized_DomainCredentails_With_Required_Fields_Should_Generate_NonEmpty_FormContent(string username, string password)
    {
      var domainCredentialRequest = new DomainCredentialRequest(username, password);

      var formContent = domainCredentialRequest.ToFormContent();

      Assert.NotEmpty(formContent);
    }

    [Theory]
    [InlineData("username", "password")]
    public void Initialized_DomainCredentails_With_Required_Fields_Should_Generate_NonEmpty_JsonObject(string username, string password)
    {
      var domainCredentialRequest = new DomainCredentialRequest(username, password);

      var json = domainCredentialRequest.ToJson();

      Assert.NotEmpty(json);
    }
  }
}