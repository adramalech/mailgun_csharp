using System;
using System.Net.Mail;
using MailgunSharp;
using MailgunSharp.Messages;
using Xunit;

namespace MailgunSharp.Test.Messages
{
  public class MessageBuilderTest
  {
    [Fact]
    public void Create_Basic_Message()
    {
      var message = new MessageBuilder().SetTestMode(true)
                                        .SetFrom(new MailAddress(@"testuser@example.com"))
                                        .AddRecipient(new Recipient(new MailAddress(@"sometestuser@example.com")))
                                        .SetSubject("Test Message")
                                        .SetTextContentBody("Test Basic Message")
                                        .Build();

      Assert.NotNull(message);
    }
  }
}