using System;
using System.Net.Mail;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using MailgunSharp.Supression;
using MailgunSharp.Enums;
using System.Collections;

namespace MailgunSharp.Test.Supression
{
  public class BounceRequestTest
  {
    [Fact]
    public void Initialized_Bounce_Request_Check_Default_Values()
    {
      var bounceRequest = new BounceRequest(new MailAddress(@"john.doe@example.com"));

      Assert.Equal(@"john.doe@example.com", bounceRequest.EmailAddress.Address);

      Assert.Equal(SmtpErrorCode.MAILBOX_UNAVAILABLE, bounceRequest.Code);

      Assert.Equal(string.Empty, bounceRequest.Error);
    }

    [Theory]
    [ClassData(typeof(BounceRequestGenerator))]
    public void Bounce_Request_Should_Set_Optional_Params(string emailAddress, SmtpErrorCode code, string error, DateTime createdAt, TimeZoneInfo tzInfo)
    {
      var bounceRequest = new BounceRequest(new MailAddress(emailAddress), code, error, createdAt, tzInfo);

      Assert.Equal(emailAddress, bounceRequest.EmailAddress.Address);

      Assert.Equal(code, bounceRequest.Code);

      Assert.Equal(error, bounceRequest.Error);
    }
  }

  public class BounceRequestGenerator : IEnumerable<object[]>
  {
    private static List<object[]> bounceRequests = new List<object[]>
    {
      new object [] { @"john.doe@example.com", SmtpErrorCode.EXCEEDS_STORAGE_ALLOCATION, "error occured", DateTime.UtcNow, null },
      new object [] { @"john.doe@example.com", SmtpErrorCode.COMMAND_NOT_IMPLEMENTED, "error occured", null, null }
    };

    public IEnumerator<object[]> GetEnumerator() => bounceRequests.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
