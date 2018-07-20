using System;
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

      Assert.True(collection.Count == 1);

      foreach (var kvp in collection)
      {
        Assert.True(key == kvp.Key);

        Assert.True(value == kvp.Value);
      }
    }

    [Theory]
    [InlineData(null, "a")]
    [InlineData("", "a")]
    [InlineData(" ", "a")]
    [InlineData("b", "")]
    [InlineData("b", null)]
    [InlineData("b", " ")]
    [InlineData("b", "")]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData(" ", " ")]
    public void KeyValuePair_Collection_AddIfNotNullEmtpyWhitespace_Add_A_Null_Empty_Whitespace_Should_Be_False(string key, string value)
    {
      var collection = new Collection<KeyValuePair<string, string>>();

      Assert.False(collection.AddIfNotNullOrEmpty(key, value));
    }

    [Theory]
    [InlineData("a", "b")]
    [InlineData("key", "value")]
    [InlineData(" k ", " v ")]
    public void KeyValuePair_Collection_AddIfNotNullEmtpyWhitespace_Add_A_Null_Empty_Whitespace_Should_Be_True(string key, string value)
    {
      var collection = new Collection<KeyValuePair<string, string>>();

      Assert.True(collection.AddIfNotNullOrEmpty(key, value));
    }
  }
}