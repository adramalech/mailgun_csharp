using System;
using System.Net.Mail;
using System.Collections.Generic;
using Xunit;
using MailgunSharp.Supression;
using MailgunSharp.Enums;
using System.Collections;
using MailgunSharp.Test.Fakes;
using NodaTime;

namespace MailgunSharp.Test.Supression
{
  public class BounceRequestTest
  {
    [Fact]
    public void Initialized_BounceRequest_Check_Default_Values()
    {
      var mailAddress = new MailAddress(@"john.doe@example.com");

      var bounceRequest = new BounceRequest(mailAddress);

      Assert.Equal(@"john.doe@example.com", bounceRequest.EmailAddress.Address);

      Assert.Equal(SmtpErrorCode.MAILBOX_UNAVAILABLE, bounceRequest.Code);

      Assert.Equal(string.Empty, bounceRequest.Error);
    }

    [Theory]
    [ClassData(typeof(BounceRequestGenerator))]
    public void BounceRequest_Should_Set_Optional_Params(string emailAddress, SmtpErrorCode code, string error, Instant createdAt)
    {
      var mailAddress = new MailAddress(emailAddress);

      var bounceRequest = new BounceRequest(mailAddress, code, error, createdAt);

      Assert.Equal(emailAddress, bounceRequest.EmailAddress.Address);

      Assert.Equal(code, bounceRequest.Code);

      Assert.Equal(error, bounceRequest.Error);
    }

    [Theory]
    [ClassData(typeof(BounceRequestGenerator))]
    public void BounceRequest_When_Converted_To_FormContent_Should_Not_Be_Empty(string emailAddress, SmtpErrorCode code, string error, Instant createdAt)
    {
      var mailAddress = new MailAddress(emailAddress);

      var bounceRequest = new BounceRequest(mailAddress, code, error, createdAt);

      var formContent = bounceRequest.ToFormContent();

      Assert.NotEmpty(formContent);
    }

    [Theory]
    [ClassData(typeof(BounceRequestGenerator))]
    public void BounceRequest_When_Converted_To_JsonObject_Should_Not_Be_Empty(string emailAddress, SmtpErrorCode code, string error, Instant createdAt)
    {
      var mailAddress = new MailAddress(emailAddress);

      var bounceRequest = new BounceRequest(mailAddress, code, error, createdAt);

      var json = bounceRequest.ToJson();

      Assert.NotEmpty(json);
    }
  }

  public class BounceRequestGenerator : IEnumerable<object[]>
  {
    private static readonly List<object[]> bounceRequests = new List<object[]>
    {
      new object [] { @"john.doe@example.com", SmtpErrorCode.EXCEEDS_STORAGE_ALLOCATION, "error occured", new FakeClock().GetCurrentInstant() },
      new object [] { @"john.doe@example.com", SmtpErrorCode.COMMAND_NOT_IMPLEMENTED, "error occured", null }
    };

    public IEnumerator<object[]> GetEnumerator() => bounceRequests.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
