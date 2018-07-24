using System.Collections.Generic;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("MailgunSharp.Test")]

namespace MailgunSharp.Extensions
{
  internal static class CollectionExtensions
  {
    /// <summary>
    /// Add a key-value string pair to a collection of key-value string pairs if key and value are not null, empty, or whitespace.
    /// </summary>
    /// <param name="collection">The collection to add the key-value string pair to.</param>
    /// <param name="key">The key of the key-value pair.</param>
    /// <param name="value">The value of the key-value pair.</param>
    /// <returns>True if the key-value string pair was added to the collection or false if it was not added.</returns>
    public static bool AddIfNotNullOrEmpty(this ICollection<KeyValuePair<string, string>> collection, string key, string value)
    {
      if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
      {
        return false;
      }

      collection.Add(new KeyValuePair<string, string>(key, value));

      return true;
    }

    /// <summary>
    /// Add a key-value pair to a collection of key-value pairs.
    /// </summary>
    /// <param name="collection">The collection to add the key-value pair to.</param>
    /// <param name="key">The key of the key-value pair.</param>
    /// <param name="value">The value of the key-value pair.</param>
    /// <typeparam name="TK">The key's type.</typeparam>
    /// <typeparam name="TV">The value's type.</typeparam>
    public static void Add<TK, TV>(this ICollection<KeyValuePair<TK, TV>> collection, TK key, TV value)
    {
      collection.Add(new KeyValuePair<TK, TV>(key, value));
    }
  }
}