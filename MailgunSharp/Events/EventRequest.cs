using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;
using MailgunSharp.Request;

namespace MailgunSharp.Events
{
  public sealed class EventRequest : IEventRequest
  {
    /// <summary>
    /// Maximum number of entries to return in the response.
    /// </summary>
    private const int MAX_RESULT_LIMIT = 300;

    /// <summary>
    /// The beginning of the search time range.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime? Begin { get; set; }

    /// <summary>
    /// The end of the search time range.
    /// </summary>
    /// <value>DateTime</value>
    public DateTime? End { get; set; }

    /// <summary>
    /// Defines the direction of the search time range and must be provided if the range end time is not specified.
    /// </summary>
    /// <value>boolean</value>
    public bool? Ascending { get; set; }

    /// <summary>
    /// Return the response in pretty json.
    /// </summary>
    /// <value>boolean</value>
    public bool? Pretty { get; set; }

    /// <summary>
    /// The number of entries to return in the response.
    ///
    /// Defaults to the maximum limit of 300.
    /// </summary>
    /// <value>Integer</value>
    public int Limit { get; set; } = MAX_RESULT_LIMIT;

    /// <summary>
    /// Message size as an int value representing number of bytes.
    /// </summary>
    /// <value>Integer</value>
    public int? Size { get; set; }

    /// <summary>
    /// The list of events to filter by.
    /// </summary>
    /// <value>List of event types.</value>
    public ICollection<EventType> EventTypes { get; set; }

    /// <summary>
    /// The attachment filename to search for.
    /// </summary>
    /// <value>string</value>
    public string AttachmentFileName { get; set; }

    /// <summary>
    /// A Mailgun message id returned by the messages API to search for.
    /// </summary>
    /// <value>string</value>
    public string MessageId { get; set; }

    /// <summary>
    /// A subject line to filter by.
    /// </summary>
    /// <value>string</value>
    public string Subject { get; set; }

    /// <summary>
    /// An email address mentioned in the "to" MIME header.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress To { get; set; }

    /// <summary>
    /// An email address mentioned in the "from" MIME header.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress From { get; set; }

    /// <summary>
    /// An email address of a particular recipient.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    public MailAddress Recipient { get; set; }

    /// <summary>
    /// User defined tags to filter by.
    /// </summary>
    /// <value>List of strings</value>
    public ICollection<string> Tags { get; set; }

    /// <summary>
    /// Used to filter events based on severity, if exists. (Currently failed events only.)
    /// </summary>
    /// <value>Severity type.</value>
    public Severity? SeverityType { get; set; }

    /// <summary>
    /// Create an instance of the Event Request class.
    /// </summary>
    public EventRequest()
    {
      this.EventTypes = new Collection<EventType>();
      this.Tags = new Collection<string>();
    }

    /// <summary>
    /// Get the event request object as a query string to be used in an http request.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The limit cannot have a minimum value less than an one or greater than maximum value 300, and message size cannot be less than 1 byte, if provided.</exception>
    /// <returns>string</returns>
    public string ToQueryString()
    {
      if (this.Limit < 1 || this.Limit > MAX_RESULT_LIMIT)
      {
        throw new ArgumentOutOfRangeException(nameof(this.Limit), this.Limit, "Limit has to be provided and cannot be less than a minimum value of 1 or a maximum value of 300!");
      }

      var queryStringBuilder = new QueryStringBuilder();

      queryStringBuilder.Append("limit", this.Limit.ToString());

      if (this.Begin.HasValue)
      {
        queryStringBuilder.Append("begin", ((DateTimeOffset)this.Begin.Value).ToUnixTimeSeconds().ToString());
      }

      if (this.End.HasValue)
      {
        queryStringBuilder.Append("end", ((DateTimeOffset)this.End.Value).ToUnixTimeSeconds().ToString());
      }

      if (this.Ascending.HasValue)
      {
        queryStringBuilder.Append("ascending",  this.Ascending.Value.ToYesNo());
      }

      if (this.Pretty.HasValue)
      {
        queryStringBuilder.Append("pretty", this.Pretty.Value.ToYesNo());
      }

      if (!this.MessageId.IsNullEmptyWhitespace())
      {
        queryStringBuilder.Append("message-id", this.MessageId);
      }

      if (this.Recipient != null)
      {
        queryStringBuilder.Append("recipient", this.Recipient.Address);
      }

      if (this.To != null)
      {
        queryStringBuilder.Append("to", this.To.Address);
      }

      if (this.Size.HasValue)
      {
        if (this.Size.Value < 1)
        {
          throw new ArgumentOutOfRangeException(nameof(this.Size), this.Size, "Message size cannot be less than 1 byte!");
        }

        queryStringBuilder.Append("size", this.Size.ToString());
      }

      if (!this.AttachmentFileName.IsNullEmptyWhitespace())
      {
        queryStringBuilder.Append("attachment", this.AttachmentFileName);
      }

      if (this.From != null)
      {
        queryStringBuilder.Append("from", this.From.Address);
      }

      if (!this.Subject.IsNullEmptyWhitespace())
      {
        queryStringBuilder.Append("subject", this.Subject);
      }

      var eventTypeCount = this.EventTypes.Count;

      if (this.EventTypes != null && eventTypeCount > 0)
      {
        var stringBuilder = new StringBuilder();

        if (eventTypeCount > 1)
        {
          stringBuilder.Append("(");
        }

        var i = 0;

        foreach (var type in this.EventTypes)
        {
          stringBuilder.Append(EnumLookup.GetEventTypeName(type));

          i++;

          if (eventTypeCount > 1 && i >= eventTypeCount)
          {
            stringBuilder.Append(" or ");
          }
        }

        if (eventTypeCount > 1)
        {
          stringBuilder.Append(")");
        }

        if (!stringBuilder.IsEmpty())
        {
          queryStringBuilder.Append("event", stringBuilder.ToString());
        }
      }

      if (this.SeverityType.HasValue)
      {
        queryStringBuilder.Append("severity", EnumLookup.GetSeverityTypeName(this.SeverityType.Value));
      }

      var tagCount = this.EventTypes.Count;

      if (this.Tags != null && tagCount > 0)
      {
        var stringBuilder = new StringBuilder();

        if (tagCount == 1)
        {
          stringBuilder.Append("");
        }
        else
        {
          stringBuilder.Append("(");
        }

        var i = 0;

        foreach (var tag in this.Tags)
        {
          stringBuilder.Append(tag);

          i++;

          if (tagCount > 1 && i >= tagCount)
          {
            stringBuilder.Append(" or ");
          }
        }

        if (tagCount > 1)
        {
          stringBuilder.Append(")");
        }

        if (!stringBuilder.IsEmpty())
        {
          queryStringBuilder.Append("tags", stringBuilder.ToString());
        }
      }

      return queryStringBuilder.ToString();
    }
  }
}
