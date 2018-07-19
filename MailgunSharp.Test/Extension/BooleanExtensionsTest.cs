using System;
using MailgunSharp.Extensions;
using Xunit;

namespace MailgunSharp.Test.Extension
{
  public class BooleanExtensionsTest
  {
    [Fact]
    public void Check_Boolean_ToYesNo_With_True_Should_Be_Yes()
    {
      var flag = true;

      var str = flag.ToYesNo();

      Assert.True(!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str));

      Assert.True(str == "yes");
    }

    [Fact]
    public void Check_Boolean_ToYesNo_With_False_Should_Be_No()
    {
      var flag = false;

      var str = flag.ToYesNo();

      Assert.True(!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str));

      Assert.True(str == "no");
    }
  }
}