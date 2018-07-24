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

      Assert.True(bounceRequest.EmailAddress.Address == @"john.doe@example.com");

      Assert.True(bounceRequest.Code == SmtpErrorCode.MAILBOX_UNAVAILABLE);

      Assert.True(bounceRequest.Error == String.Empty);
    }

    [Theory]
    [ClassData(typeof(BounceRequestGenerator))]
    public void Bounce_Request_Should_Set_Optional_Params(string emailAddress, SmtpErrorCode code, string error, DateTime createdAt, TimeZoneInfo tzInfo)
    {
      var bounceRequest = new BounceRequest(new MailAddress(emailAddress), code, error, createdAt, tzInfo);

      Assert.True(bounceRequest.EmailAddress.Address == emailAddress);

      Assert.True(bounceRequest.Code == code);

      Assert.True(bounceRequest.Error == error);
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
