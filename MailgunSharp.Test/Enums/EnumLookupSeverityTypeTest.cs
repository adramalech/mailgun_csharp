using System;
using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupSeverityTypeTest
  {
    [Fact]
    public void Check_Severity_Type_Permanent_Should_Return_string_permanent()
    {
      var type = Severity.PERMANENT;

      var name = EnumLookup.GetSeverityTypeName(type);

      Assert.True(name == "permanent");
    }

    [Fact]
    public void Check_Severity_Type_Temporary_Should_Return_string_temporary()
    {
      var type = Severity.TEMPORARY;

      var name = EnumLookup.GetSeverityTypeName(type);

      Assert.True(name == "temporary");
    }
  }
}