using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupTimeResolutionTest
  {
    [Fact]
    public void Check_TimeResolution_Hour_Should_Return_String_h()
    {
      var type = TimeResolution.HOUR;

      var name = EnumLookup.GetTimeResolutionName(type);

      Assert.Equal("h", name);
    }

    [Fact]
    public void Check_TimeResolution_Day_Should_Return_String_d()
    {
      var type = TimeResolution.DAY;

      var name = EnumLookup.GetTimeResolutionName(type);

      Assert.Equal("d", name);
    }

    [Fact]
    public void Check_TimeResolution_Month_Should_Return_String_m()
    {
      var type = TimeResolution.MONTH;

      var name = EnumLookup.GetTimeResolutionName(type);

      Assert.Equal("m", name);
    }
  }
}