using System;
using Xunit;
using MailgunSharp.Request;

namespace MailgunSharp.Test.Request
{
  public class QueryStringTest
  {
    [Fact]
    public void Initialized_QueryString_Should_Have_Count_Equal_To_Zero()
    {
      var queryString = new QueryString();

      Assert.True(queryString != null);

      Assert.True(queryString.Count == 0);
    }

    [Theory]
    [InlineData("var1", "val1")]
    public void QueryString_AppendParam_Check_If_Exists_Should_Have_Count_Equal_To_One(string variable, string value)
    {
      var queryString = new QueryString();

      var wasAppended = queryString.AppendIfNotNullOrEmpty(variable, value);

      Assert.True(wasAppended);

      Assert.True(queryString.Count == 1);
    }

    [Theory]
    [InlineData("", "var1")]
    [InlineData("var1", "")]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData(" ", "")]
    [InlineData("", " ")]
    [InlineData(" ", " ")]
    public void QueryString_AppendParam_Should_Throw_Error_If_Variable_Or_Value_Is_Empty_Or_Null(string variable, string value)
    {
      var queryString = new QueryString();

      Assert.Throws<ArgumentNullException>(() => queryString.AppendIfNotNullOrEmpty(variable, value));
    }
  }
}