using System;
using Xunit;
using MailgunSharp.MailingLists;
using Newtonsoft.Json;

namespace MailgunSharp.Test.MailingLists
{
  public class MemberTest
  {
    [Fact]
    public void Initialized_Member_With_Null_Address_Should_Throw_Exception()
    {
      Assert.Throws<ArgumentNullException>(() => {
        var member = new Member(null);
      });
    }

    [Fact]
    public void Initialized_Member_With_Null_Address_And_Set_Optional_Fields_Should_Throw_Exception()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var member = new Member(null, "name");
      });
    }

    [Theory]
    [InlineData(@"myaddress@sample.com")]
    public void Initialized_Member_Should_Only_Require_Address(string emailAddress)
    {
      var member = new Member(emailAddress);

      Assert.Equal(emailAddress, member.EmailAddress.Address);
    }

    [Theory]
    [InlineData(@"address@eample.com", "firstname lastname")]
    public void Initialized_Member_With_Optional_Fields_Should_Be_Same_Values(string emailAddress, string name)
    {
      var member = new Member(emailAddress, name);

      Assert.Equal(name, member.Name);
    }

    [Theory]
    [InlineData(@"address@eample.com", "firstname lastname")]
    public void Initialized_Member_With_Optional_Fields_Should_Generate_NonEmpty_FormContent(string emailAddress, string name)
    {
      var member = new Member(emailAddress, name);

      var formContent = member.ToFormContent();

      Assert.NotEmpty(formContent);
    }

    [Theory]
    [InlineData(@"address@sample.com", "firstname lastname")]
    public void Initialized_Member_With_Optional_Fields_Should_Generate_NonEmpty_JsonObject(string emailAddress, string name)
    {
      var member = new Member(emailAddress, name);

      var json = member.ToJson();

      Assert.NotEmpty(json);
    }
  }
}