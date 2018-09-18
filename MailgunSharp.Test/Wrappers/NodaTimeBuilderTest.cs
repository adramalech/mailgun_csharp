using System;
using MailgunSharp.Test.Fakes;
using Xunit;
using MailgunSharp.Wrappers;

namespace MailgunSharp.Test.Wrappers
{
  public class NodaTimeBuilderTest
  {
    private readonly FakeClock clock;

    public NodaTimeBuilderTest()
    {
      this.clock = new FakeClock();
    }

    [Fact]
    public void Build_NodaTime_DateTime()
    {
      var builder = new NodaTimeBuilder(this.clock);

      var dt = builder
                  .SetDateTimeUtc(1970, 1, 1, 0, 0, 0)
                  .AddDays(1)
                  .AddHours(1)
                  .AddMinutes(1)
                  .AddSeconds(1)
                  .Build();

      Assert.Equal(DateTimeKind.Utc, dt.Kind);
      Assert.Equal(1970, dt.Year);
      Assert.Equal(1, dt.Month);
      Assert.Equal(2, dt.Day);
      Assert.Equal(1, dt.Hour);
      Assert.Equal(1, dt.Minute);
      Assert.Equal(1, dt.Second);
      Assert.Equal(0, dt.Millisecond);
    }

    [Fact]
    public void NodaTimeBuilder_Should_Allow_Fake_Clock()
    {
      var builder = new NodaTimeBuilder(this.clock);

      var dt = builder
                  .AddDays(1)
                  .AddHours(1)
                  .AddMinutes(1)
                  .AddSeconds(1)
                  .Build();

      Assert.Equal(DateTimeKind.Utc, dt.Kind);
      Assert.Equal(1970, dt.Year);
      Assert.Equal(1, dt.Month);
      Assert.Equal(2, dt.Day);
      Assert.Equal(1, dt.Hour);
      Assert.Equal(1, dt.Minute);
      Assert.Equal(1, dt.Second);
      Assert.Equal(0, dt.Millisecond);
    }

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void NodaTimeBuilder_Should_Throw_Exception_When_DateTimeKind_NotUtc(DateTimeKind kind)
    {
      Assert.Throws<ArgumentException>(() => {
        var dateTimeLocal = new NodaTimeBuilder(this.clock)
                              .SetDateTimeUtc(new DateTime(1970, 1, 1, 1, 1, 1, kind))
                              .Build();
      });
    }
  }
}