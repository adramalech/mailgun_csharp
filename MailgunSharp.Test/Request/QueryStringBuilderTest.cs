using System;
using Xunit;
using MailgunSharp.Request;
using MailgunSharp.Extensions;

namespace MailgunSharp.Test.Request
{
  public class QueryStringTest
  {
    [Fact]
    public void Initialized_QueryString_Should_Have_Count_Equal_To_Zero()
    {
      var queryStringBuilder = new QueryStringBuilder();

      Assert.True(queryStringBuilder != null);

      Assert.True(queryStringBuilder.Count == 0);
    }

    [Theory]
    [InlineData("var1", "val1")]
    public void QueryString_AppendParam_Check_If_Exists_Should_Have_Count_Equal_To_One(string variable, string value)
    {
      var queryStringBuilder = new QueryStringBuilder();

      queryStringBuilder.Append(variable, value);

      Assert.True(queryStringBuilder.Count == 1);
    }

    [Theory]
    [InlineData("", "var1")]
    [InlineData("var1", "")]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData(" ", "")]
    [InlineData("", " ")]
    [InlineData(" ", " ")]
    public void QueryStringBuilder_AppendParam_Should_Throw_Error_If_Variable_Or_Value_Is_Empty_Or_Null(string variable, string value)
    {
      var queryStringBuilder = new QueryStringBuilder();

      Assert.Throws<ArgumentNullException>(() => queryStringBuilder.Append(variable, value));
    }

    [Fact]
    public void QueryStringBuilder_AppendParam_Should_Have_Correct_Formatted_ToString()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder
                          .Append("var1", "val1")
                          .ToString();

      Assert.True(queryString.IndexOf('?') == 0);

      Assert.True(queryString.Contains('='));

      Assert.False(queryString.Contains('&'));
    }

    [Fact]
    public void QueryStringBuilder_AppendMultipleParam_Should_Have_Correct_Formatted_ToString()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder
                          .Append("var1", "val1")
                          .Append("var2", "val2")
                          .ToString();

      Assert.True(queryString.IndexOf('?') == 0);

      var equalStrSplit = queryString.Split('=');

      Assert.True(equalStrSplit != null && equalStrSplit.Length == 3);

      Assert.True(queryString.Contains('&'));
    }
  }
}
