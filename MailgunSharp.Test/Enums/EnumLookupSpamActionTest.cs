using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupSpamActionTest
  {
    [Fact]
    public void Check_SpamAction_Blocked_Should_Return_String_blocked()
    {
      var type = SpamAction.BLOCKED;

      var name = EnumLookup.GetSpamActionName(type);

      Assert.True(name == "blocked");
    }

    [Fact]
    public void Check_SpamAction_Disabled_Should_Return_String_disabled()
    {
      var type = SpamAction.DISABLED;

      var name = EnumLookup.GetSpamActionName(type);

      Assert.True(name == "disabled");
    }

    [Fact]
    public void Check_SpamAction_Tag_Should_Return_String_tag()
    {
      var type = SpamAction.TAG;

      var name = EnumLookup.GetSpamActionName(type);

      Assert.True(name == "tag");
    }
  }
}