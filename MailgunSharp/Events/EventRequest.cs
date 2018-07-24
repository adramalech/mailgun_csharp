using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;

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
    /// <returns>string</returns>
    public string ToQueryString()
    {
      var strBuilder = new StringBuilder();

      if (this.Limit < 1 || this.Limit > MAX_RESULT_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit has to be provided and cannot be less than 1!");
      }

      strBuilder.Append($"limit={this.Limit}");

      if (this.Begin.HasValue)
      {
        strBuilder.Append($"&begin={((DateTimeOffset)this.Begin.Value).ToUnixTimeSeconds()}");
      }

      if (this.End.HasValue)
      {
        strBuilder.Append($"&end={((DateTimeOffset)this.End.Value).ToUnixTimeSeconds()}");
      }

      if (this.Ascending.HasValue)
      {
        strBuilder.Append($"&ascending={this.Ascending.Value.ToYesNo()}");
      }

      if (this.Pretty.HasValue)
      {
        strBuilder.Append($"&pretty={this.Pretty.Value.ToYesNo()}");
      }

      if (this.Size.HasValue)
      {
        strBuilder.Append($"size={this.Size}");
      }

      if (!this.MessageId.IsNullEmptyWhitespace())
      {
        strBuilder.Append($"&message-id={this.MessageId}");
      }

      if (this.Recipient != null)
      {
        strBuilder.Append($"&recipient={this.Recipient.Address}");
      }

      if (this.To != null)
      {
        strBuilder.Append($"&to={this.To.Address}");
      }

      if (this.Size.HasValue)
      {
        if (this.Size.Value < 1)
        {
          throw new ArgumentOutOfRangeException("Message size cannot be less than 1 byte!");
        }

        strBuilder.Append($"&size={this.Size}");
      }

      if (!this.AttachmentFileName.IsNullEmptyWhitespace())
      {
        strBuilder.Append($"&attachment={this.AttachmentFileName}");
      }

      if (this.From != null)
      {
        strBuilder.Append($"&from={this.From.Address}");
      }

      if (!this.Subject.IsNullEmptyWhitespace())
      {
        strBuilder.Append($"&subject={this.Subject}");
      }

      var eventTypeCount = this.EventTypes.Count;

      if (this.EventTypes != null && eventTypeCount > 0)
      {
        if (eventTypeCount == 1)
        {
          strBuilder.Append("&event=");
        }
        else
        {
          strBuilder.Append("&event=(");
        }

        var i = 0;

        foreach (var type in this.EventTypes)
        {
          strBuilder.Append(EnumLookup.GetEventTypeName(type));

          i++;

          if (eventTypeCount > 1 && i >= eventTypeCount)
          {
            strBuilder.Append(" or ");
          }
        }

        if (eventTypeCount > 1)
        {
          strBuilder.Append(")");
        }
      }

      if (this.SeverityType.HasValue)
      {
        strBuilder.Append($"&severity={EnumLookup.GetSeverityTypeName(this.SeverityType.Value)}");
      }

      var tagCount = this.EventTypes.Count;

      if (this.Tags != null && tagCount > 0)
      {
        if (tagCount == 1)
        {
          strBuilder.Append("&tags=");
        }
        else
        {
          strBuilder.Append("&tags=(");
        }

        var i = 0;

        foreach (var tag in this.Tags)
        {
          strBuilder.Append(tag);

          i++;

          if (tagCount > 1 && i >= tagCount)
          {
            strBuilder.Append(" or ");
          }
        }

        if (tagCount > 1)
        {
          strBuilder.Append(")");
        }
      }

      return strBuilder.ToString();
    }
  }
}
