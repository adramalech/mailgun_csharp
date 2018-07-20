using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using Newtonsoft.Json;
using MailgunSharp.Supression;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Supression
{
  public class BounceRequestTest
  {
    [Theory]
    [InlineData(@"john.doe@example.com")]
    public void Initialized_Bounce_Request_Check_Default_Values(string emailAddress)
    {
      var bounceRequest = new BounceRequest(new MailAddress(emailAddress));

      Assert.True(bounceRequest.EmailAddress.Address == emailAddress);

      Assert.True(bounceRequest.Code == SmtpErrorCode.MAILBOX_UNAVAILABLE);

      Assert.True(bounceRequest.Error == String.Empty);
    }
  }
}
