using Xunit;
using MailgunSharp.Extensions;
using MailgunSharp.Test.Fakes;
using MailgunSharp.Wrappers;

namespace MailgunSharp.Test.Extension
{
  public class NodaTimeExtensionsTest
  {
    [Fact]
    public void NodaTimeWrapper_ShouldProduceEpochString()
    {
      var instant = new NodaTimeBuilder(new FakeClock()).Build();

      var str = instant.ToRfc2822DateFormat();

      Assert.NotNull(str);
    }
  }
}