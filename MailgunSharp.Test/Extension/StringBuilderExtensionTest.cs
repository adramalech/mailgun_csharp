using System.Text;
using Xunit;
using MailgunSharp.Extensions;

namespace MailgunSharp.Test.Extension
{
  public class StringBuilderExtensionTest
  {
    [Fact]
    public void StringBuilder_Initialized_Should_Be_Empty()
    {
      var stringBuilder = new StringBuilder();

      Assert.True(stringBuilder.IsEmpty());
    }

    [Fact]
    public void StringBuilder_Initialized_With_Append_Should_Not_Be_Empty()
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.Append("abc123");

      Assert.False(stringBuilder.IsEmpty());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void StringBuilder_Append_NullString_EmptyString_WhiteSpaceString_Should_Be_False_And_Not_Added(string str)
    {
      var stringBuilder = new StringBuilder();

      Assert.False(stringBuilder.AddIfNotNullEmptyWhitespace(str));

      Assert.True(stringBuilder.Length == 0);
    }

    [Theory]
    [InlineData("a")]
    [InlineData(" a ")]
    [InlineData("abc123 ")]
    [InlineData(" abc123")]
    public void StringBuilder_Append_String_Should_Be_True_And_Have_One_ElementAdded(string str)
    {
      var stringBuilder = new StringBuilder();

      Assert.True(stringBuilder.AddIfNotNullEmptyWhitespace(str));

      Assert.False(stringBuilder.Length == 0);
    }
  }
}