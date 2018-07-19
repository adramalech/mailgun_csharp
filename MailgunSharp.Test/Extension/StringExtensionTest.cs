using System;
using Xunit;
using MailgunSharp.Extensions;

namespace MailgunSharp.Test.Extension
{
  public class StringExtensionTest
  {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Check_String_If_Null_Empty_Whitespace_Should_Be_True(string str)
    {
      Assert.True(str.IsNullEmptyWhitespace());
    }

    [Theory]
    [InlineData(" a ")]
    [InlineData("abc")]
    public void Check_String_If_Not_Null_Empty_Whitspace_Should_Be_False(string str)
    {
      Assert.False(str.IsNullEmptyWhitespace());
    }
  }
}