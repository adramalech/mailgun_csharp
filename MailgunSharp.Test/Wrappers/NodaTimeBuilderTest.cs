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
      var builder = new NodaTimeBuilder(1970, 1, 1, 0, 0, 0);

      var instant = builder
                  .AddDays(1)
                  .AddHours(1)
                  .AddMinutes(1)
                  .AddSeconds(1)
                  .Build();

      var dt = instant.ToDateTimeUtc();

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

      var instant = builder
                  .AddDays(1)
                  .AddHours(1)
                  .AddMinutes(1)
                  .AddSeconds(1)
                  .Build();

      var dt = instant.ToDateTimeUtc();

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
        var dateTimeLocal = new NodaTimeBuilder(new FakeDateTime(kind).Now).Build();
      });
    }

    [Fact]
    public void NodaTimeBuilder_If_Add_Add_Subtract_Same_Value_Days_Should_Not_Change_Date()
    {
      var instant = new NodaTimeBuilder(this.clock)
                          .AddDays(1)
                          .SubtractDays(1)
                          .Build();

      var dt = instant.ToDateTimeUtc();

      Assert.Equal(DateTimeKind.Utc, dt.Kind);
      Assert.Equal(1970, dt.Year);
      Assert.Equal(1, dt.Month);
      Assert.Equal(1, dt.Day);
      Assert.Equal(0, dt.Hour);
      Assert.Equal(0, dt.Minute);
      Assert.Equal(0, dt.Second);
      Assert.Equal(0, dt.Millisecond);
    }

    [Fact]
    public void NodaTimeBuilder_Initialize_UTC_Datetime_Now_Subtract_Day_Should_Rollback_To_LastDay_Previous_Year()
    {
      //1970-01-01 00:00:00.000 to 1969-12-31 00:00:00.000.
      var instant = new NodaTimeBuilder(new FakeDateTime().UtcNow)
                  .SubtractDays(1)
                  .Build();

      var dt = instant.ToDateTimeUtc();

      Assert.Equal(1969, dt.Year);
      Assert.Equal(12, dt.Month);
      Assert.Equal(31, dt.Day);
    }
  }
}