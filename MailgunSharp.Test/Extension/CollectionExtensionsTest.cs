using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using MailgunSharp.Extensions;

namespace MailgunSharp.Test.Extension
{
  public class CollectionExtensionsTest
  {
    [Theory]
    [InlineData("key", "value")]
    public void KeyValuePair_Collection_Add_A_KeyValuePair_Should_Exist(string key, string value)
    {
      var collection = new Collection<KeyValuePair<string, string>>();

      collection.Add(key, value);

      Assert.NotEmpty(collection);

      foreach (var kvp in collection)
      {
        Assert.Equal(key, kvp.Key);

        Assert.Equal(value,  kvp.Value);
      }
    }

    [Theory]
    [InlineData(null, "a")]
    [InlineData("", "a")]
    [InlineData(" ", "a")]
    [InlineData("b", "")]
    [InlineData("b", null)]
    [InlineData("b", " ")]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData(" ", " ")]
    public void KeyValuePair_Collection_AddIfNotNullEmtpyWhitespace_Add_A_Null_Empty_Whitespace_Should_Be_False(string key, string value)
    {
      var collection = new Collection<KeyValuePair<string, string>>();

      var flag = collection.AddIfNotNullOrEmpty(key, value);

      Assert.False(flag);
    }

    [Theory]
    [InlineData("key", "value")]
    [InlineData(" key ", " value ")]
    public void KeyValuePair_Collection_AddIfNotNullEmtpyWhitespace_Add_A_Null_Empty_Whitespace_Should_Be_True(string key, string value)
    {
      var collection = new Collection<KeyValuePair<string, string>>();

      var flag = collection.AddIfNotNullOrEmpty(key, value);

      Assert.True(flag);
    }
  }
}
