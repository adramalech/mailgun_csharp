using System;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Events
{
  public sealed class EventRequest : IEventRequest
  {
    private const int MAX_RESULT_LIMIT = 300;

    public DateTime? Begin { get; set; }
    public DateTime? End { get; set; }
    public bool? Ascending { get; set; }
    public bool? Pretty { get; set; }
    public int Limit { get; set; } = MAX_RESULT_LIMIT;
    public int? Size { get; set; }
    public ICollection<EventType> EventTypes { get; set; }
    public string AttachmentFileName { get; set; }
    public string MessageId { get; set; }
    public string Subject { get; set; }
    public MailAddress To { get; set; }
    public MailAddress From { get; set; }
    public MailAddress Recipient { get; set; }
    public ICollection<string> Tags { get; set; }
    public Severity? SeverityType { get; set; }

    public string ToQueryString()
    {
      var collection = new NameValueCollection();

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
        strBuilder.Append($"&ascending={boolToYesNo(this.Ascending.Value)}");
      }

      if (this.Pretty.HasValue)
      {
        strBuilder.Append($"&pretty={boolToYesNo(this.Pretty.Value)}");
      }

      if (this.Size.HasValue)
      {
        strBuilder.Append($"size={this.Size}");
      }

      if (!checkStringIfNullEmptyWhitespace(this.MessageId))
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

      if (!checkStringIfNullEmptyWhitespace(this.AttachmentFileName))
      {
        strBuilder.Append($"&attachment={this.AttachmentFileName}");
      }

      if (this.From != null)
      {
        strBuilder.Append($"&from={this.From.Address}");
      }

      if (!checkStringIfNullEmptyWhitespace(this.Subject))
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
          strBuilder.Append(getEventTypeName(type));

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
        strBuilder.Append($"&severity={getSeverityTypeName(this.SeverityType.Value)}");
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

      return collection.ToString();
    }

    private string getEventTypeName(EventType eventType)
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

    private string getSeverityTypeName(Severity severityType)
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

    private string boolToYesNo(bool flag)
    {
      return (flag) ? "yes" : "no";
    }

    private bool checkStringIfNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
