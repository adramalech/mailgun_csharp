using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using MailgunSharp.Request;

namespace MailgunSharp.Test.Request
{
  public class QueryStringTest
  {
    [Fact]
    public void Initialized_QueryString_Should_Have_Count_Equal_To_Zero()
    {
      var queryStringBuilder = new QueryStringBuilder();

      Assert.Equal(0, queryStringBuilder.Count);
    }

    [Theory]
    [InlineData("var1", "val1")]
    public void QueryString_AppendParam_Check_If_Exists_Should_Have_Count_Equal_To_One(string variable, string value)
    {
      var queryStringBuilder = new QueryStringBuilder();

      queryStringBuilder.Append(variable, value);

      Assert.Equal(1, queryStringBuilder.Count);
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

      Assert.Throws<ArgumentNullException>(() =>
      {
        queryStringBuilder.Append(variable, value);
      });
    }

    [Fact]
    public void QueryStringBuilder_AppendParam_Should_Start_With_QuestionMarkSymbol()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder.Append("var1", "val1")
                                          .Build();

      Assert.StartsWith("?", queryString);
    }

    [Fact]
    public void QueryStringBuilder_AppendMultipleParam_Should_Have_Correct_Amount_Of_Equal_Symbols()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder.Append("var1", "val1")
                                          .Append("var2", "val2")
                                          .Append("var3", "val3")
                                          .ToString();

      var countEqualSymbol = queryString.Count(q => q == '=');

      Assert.Equal(3, countEqualSymbol);
    }

    [Fact]
    public void QueryStringBuilder_AppendMultipleParam_Should_Have_Correct_Amount_Of_Ampersand_Symbols()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder.Append("var1", "val1")
                                          .Append("var2", "val2")
                                          .Append("var3", "val3")
                                          .ToString();

      var countAmpersandSymbol = queryString.Count(q => q == '&');

      Assert.Equal(2, countAmpersandSymbol);
    }

    [Fact]
    public void QueryStringBuilder_Build_Empty_QueryString_Should_Throw_Error()
    {
      var queryStringBuilder = new QueryStringBuilder();

      Assert.Throws<InvalidOperationException>(() =>
      {
        var str = queryStringBuilder.Build();
      });
    }
  }
}
