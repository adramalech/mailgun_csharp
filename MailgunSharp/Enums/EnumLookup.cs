namespace MailgunSharp.Enums
{
  public static class EnumLookup
  {
    /// <summary>
    /// Get the description name of the event type.
    /// </summary>
    /// <param name="eventType">The event type to get the description of.</param>
    /// <returns>string</returns>
    public static string GetEventTypeName(EventType eventType)
    {
      var name = "";

      switch (eventType)
      {
        case EventType.ACCEPTED:
          name = "accepted";
          break;

        case EventType.CLICKED:
          name = "clicked";
          break;

        case EventType.COMPLAINED:
          name = "complained";
          break;

        case EventType.DELIVERED:
          name = "delivered";
          break;

        case EventType.FAILED:
          name = "failed";
          break;

        case EventType.OPENED:
          name = "opened";
          break;

        case EventType.STORED:
          name = "stored";
          break;

        case EventType.UNSUBSCRIBED:
          name = "unsubscribed";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the Time Resolution's name.
    /// </summary>
    /// <param name="resolution">The time resolution type.</param>
    /// <returns>The name of the time resolution parameter as a string.</returns>
    public static string GetTimeResolutionName(TimeResolution resolution)
    {
      var name = "";

      switch (resolution)
      {
        case TimeResolution.HOUR:
          name = "h";
          break;

        case TimeResolution.DAY:
          name = "d";
          break;

        case TimeResolution.MONTH:
          name = "m";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the Domain click tracking type name.
    /// </summary>
    /// <param name="active">The domain click tracking type.</param>
    /// <returns>The name of the domain click tracking type as a string.</returns>
    public static string GetDomainClickTrackingActiveName(DomainClickTrackingActive active)
    {
      var name = "";

      switch (active)
      {
        case DomainClickTrackingActive.HTML_ONLY:
          name = "htmlonly";
          break;

        case DomainClickTrackingActive.NO:
          name = "no";
          break;

        case DomainClickTrackingActive.YES:
          name = "yes";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the spam action name description.
    /// </summary>
    /// <param name="spamAction">The spam action type to get the description of.</param>
    /// <returns>The spam action type name as a string.</returns>
    public static string GetSpamActionName(SpamAction spamAction)
    {
      var name = "";

      switch (spamAction)
      {
        case SpamAction.BLOCKED:
          name = "blocked";
          break;

        case SpamAction.DISABLED:
          name = "disabled";
          break;

        case SpamAction.TAG:
          name = "tag";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the severity type description.
    /// </summary>
    /// <param name="severityType">The severity type to get the description of.</param>
    /// <returns>The Failed event severity type as a name.</returns>
    public static string GetSeverityTypeName(Severity severityType)
    {
      var name = "";

      switch (severityType)
      {
        case Severity.PERMANENT:
          name = "permanent";
          break;

        case Severity.TEMPORARY:
          name = "temporary";
          break;
      }

      return name;
    }

    /// <summary>
    /// Get the name of the access level type.
    /// </summary>
    /// <param name="accessLevel">The access level type.</param>
    /// <returns>Name of the access level type.</returns>
    public static string GetAccessLevelName(AccessLevel accessLevel)
    {
      var name = "";

      switch (accessLevel)
      {
        case AccessLevel.READ_ONLY:
          name = "readonly";
          break;
        case AccessLevel.MEMBERS:
          name = "members";
          break;
        case AccessLevel.EVERYONE:
          name = "everyone";
          break;
      }

      return name;
    }
  }
}