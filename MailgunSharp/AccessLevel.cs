namespace MailgunSharp
{
  /// <summary>
  /// Mailing Lists have access levels that define how users can interact with the mailing list.
  /// </summary>
  public enum AccessLevel
  {
    /// <summary>
    /// Only authenticated users can post to this list. It is used for mass announcements and newsletters.
    /// This is the default access level.
    /// </summary>
    READ_ONLY,

    /// <summary>
    /// Subscribed members of the list can communicate with each other.
    /// </summary>
    MEMBERS,

    /// <summary>
    /// Everyone can post to this list.
    /// </summary>
    EVERYONE
  }
}
