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

      string name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "readonly");
    }

    [Fact]
    public void Check_Access_Level_Members_Should_Return_String_members()
    {
      AccessLevel type = AccessLevel.MEMBERS;

      string name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "members");
    }

    [Fact]
    public void Check_Access_Level_Everyone_Should_Return_String_everyone()
    {
      AccessLevel type = AccessLevel.EVERYONE;

      string name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "everyone");
    }
  }
}