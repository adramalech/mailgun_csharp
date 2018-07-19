using System;
using MailgunSharp.Request;
using MailgunSharp.Extensions;
using Xunit;

namespace MailgunSharp.Test.Request
{
  public class QueryStringBuilderTest
  {
    [Fact]
    public void Initialize_QueryStringBuilder_Should_Return_Zero_Count_QueryString()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder.Build();

      Assert.True(queryString != null);

      Assert.True(queryString.Count == 0);
    }

    [Theory]
    [InlineData("var1", "var2")]
    [InlineData("a", "b")]
    public void Append_Parameter_To_QueryString_Shoul_Return_True_And_Have_Count_One_QueryString(string variable, string value)
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder
                          .Append(variable, value)
                          .Build();

      Assert.True(queryString != null);

      Assert.True(queryString.Count == 1);
    }

    [Fact]
    public void Append_Multiple_Parameters_Count_Of_QueryString_Should_Be_The_Same_As_List_Of_Params()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder
                          .Append("var1", "val1")
                          .Append("var2", "val2")
                          .Append("var3", "val3")
                          .Build();

      Assert.True(queryString != null);

      Assert.True(queryString.Count == 3);
    }

    [Fact]
    public void Append_MultipleParameters_Check_QueryString_ToString_Format_For_Seperators()
    {
      var queryStringBuilder = new QueryStringBuilder();

      var queryString = queryStringBuilder
                          .Append("var1", "val1")
                          .Append("var2", "val2")
                          .Append("var3", "val3")
                          .Build();

      Assert.True(queryString != null);

      Assert.True(queryString.Count == 3);

      var str = queryString.ToString();

      Assert.True(!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str));

      Assert.True(str.IndexOf('?') == 0);

      var strArray = str.Split('&');

      Assert.True(strArray != null && strArray.Length == 3);
    }
  }
}