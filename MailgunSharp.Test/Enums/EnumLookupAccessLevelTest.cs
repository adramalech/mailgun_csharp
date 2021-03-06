using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupAccessLevelTest
  {
    [Fact]
    public void Check_Access_Level_Read_Only_Should_Return_String_readonly()
    {
      AccessLevel type = AccessLevel.READ_ONLY;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.Equal("readonly", name);
    }

    [Fact]
    public void Check_Access_Level_Members_Should_Return_String_members()
    {
      AccessLevel type = AccessLevel.MEMBERS;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.Equal("members", name);
    }

    [Fact]
    public void Check_Access_Level_Everyone_Should_Return_String_everyone()
    {
      AccessLevel type = AccessLevel.EVERYONE;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.Equal("everyone", name);
    }
  }
}