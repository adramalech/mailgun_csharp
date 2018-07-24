using System;
using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupAccessLevelTest
  {
    [Fact]
    public void Check_Access_Level_Read_Only_Should_Return_String_readonly()
    {
      var type = AccessLevel.READ_ONLY;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "readonly");
    }

    [Fact]
    public void Check_Access_Level_Members_Should_Return_String_members()
    {
      var type = AccessLevel.MEMBERS;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "members");
    }

    [Fact]
    public void Check_Access_Level_Everyone_Should_Return_String_everyone()
    {
      var type = AccessLevel.EVERYONE;

      var name = EnumLookup.GetAccessLevelName(type);

      Assert.True(name == "everyone");
    }
  }
}