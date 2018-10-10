using System;
using Xunit;
using System.Net.Mail;
using MailgunSharp.MailingLists;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.MailingLists
{
  public class MailingListTest
  {
    [Fact]
    public void Initialized_MailingList_With_Null_Address_Should_Throw_Exception()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var list = new MailingList(null);
      });
    }

    [Theory]
    [InlineData("Test Name", "Test description", AccessLevel.MEMBERS)]
    public void Initialized_MailingList_With_Null_Address_And_Optional_Fields_Should_Throw_Exception(string name, string description, AccessLevel accessLevel)
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var list = new MailingList(null, name, description, accessLevel);
      });
    }

    [Theory]
    [InlineData(@"john.doe@example.com")]
    public void Initialized_MailingList_Should_Only_Require_EmailAddress(string emailAddress)
    {
      var list = new MailingList(new MailAddress(emailAddress));

      Assert.Equal(emailAddress, list.EmailAddress.Address);
    }

    [Theory]
    [InlineData(@"john.doe@example.com", "Test Name", "test description", AccessLevel.EVERYONE)]
    public void Initialized_MailingList_With_Optional_Fields_Should_Have_Same_Values(string emailAddress, string name, string description, AccessLevel accessLevel)
    {
      var list = new MailingList(new MailAddress(emailAddress), name, description, accessLevel);

      Assert.Equal(emailAddress, list.EmailAddress.Address);
      Assert.Equal(name, list.Name);
      Assert.Equal(description, list.Description);
      Assert.Equal(accessLevel, list.AccessLevel);
    }

    [Theory]
    [InlineData(@"john.doe@example.com", "Test Name", "test description", AccessLevel.EVERYONE)]
    public void Initialized_MailingList_With_Optional_Fields_Should_Produce_A_NonEmpty_FormContent(string emailAddress, string name, string description, AccessLevel accessLevel)
    {
      var list = new MailingList(new MailAddress(emailAddress), name, description, accessLevel);

      var formContent = list.ToFormContent();

      Assert.NotEmpty(formContent);
    }

    [Theory]
    [InlineData(@"john.doe@example.com", "Test Name", "test description", AccessLevel.EVERYONE)]
    public void Initialized_MailingList_With_Optional_Fields_Should_Produce_A_NonEmpty_JsonObject(string emailAddress, string name, string description, AccessLevel accessLevel)
    {
      var list = new MailingList(new MailAddress(emailAddress), name, description, accessLevel);

      var json = list.ToJson();

      Assert.NotEmpty(json);
    }
  }
}