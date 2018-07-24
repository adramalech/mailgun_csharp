using MailgunSharp.Enums;
using Xunit;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupDomainClickTrackingActiveTest
  {
    [Fact]
    public void Check_DomainClickTrackingActive_HtmlOnly_Should_Return_String_htmlonly()
    {
      var type = DomainClickTrackingActive.HTML_ONLY;

      var name = EnumLookup.GetDomainClickTrackingActiveName(type);

      Assert.True(name == "htmlonly");
    }

    [Fact]
    public void Check_DomainClickTrackingActive_No_Should_Return_String_no()
    {
      var type = DomainClickTrackingActive.NO;

      var name = EnumLookup.GetDomainClickTrackingActiveName(type);

      Assert.True(name == "no");
    }

    [Fact]
    public void Check_DomainClickTrackingActive_Yes_Should_Return_String_yes()
    {
      var type = DomainClickTrackingActive.YES;

      var name = EnumLookup.GetDomainClickTrackingActiveName(type);

      Assert.True(name == "yes");
    }
  }
}