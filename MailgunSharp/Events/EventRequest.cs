using System;
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

      if (this.Begin.HasValue) 
      {
        collection["begin"] = ((DateTimeOffset)this.Begin.Value).ToUnixTimeSeconds().ToString();
      }

      if (this.End.HasValue)
      {
        collection["end"] = ((DateTimeOffset)this.Begin.Value).ToUnixTimeSeconds().ToString();
      }

      if (this.Ascending.HasValue)
      {
        collection["ascending"] = boolToYesNo(this.Ascending.Value);
      }

      if (this.Pretty.HasValue)
      {
        collection["pretty"] = boolToYesNo(this.Pretty.Value);
      }

      if (this.Limit < 1 || this.Limit > MAX_RESULT_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit has to be provided and cannot be less than 1!");
      }
      else 
      {
        collection["limit"] = this.Limit.ToString();
      }

      if (this.Size.HasValue)
      {
        collection["size"] = this.Size.ToString();
      }

      if (this.EventTypes != null)
      {
        foreach (var type in this.EventTypes)
        {
          collection["event"] = getEventTypeName(type);
        }
      }

      if (this.Tags != null)
      {
        foreach (var tag in Tags)
        {
          collection["tag"] = tag;
        }
      }

      if (!checkStringIfNullEmptyWhitespace(this.MessageId))
      {
        collection["message-id"] = this.MessageId;
      }

      if (this.Recipient != null)
      {
        collection["recipient"] = this.Recipient.Address;
      }

      if (this.To != null)
      {
        collection["to"] = this.To.Address;
      }

      if (this.Size.HasValue)
      {
        if (this.Size.Value < 1)
        {
          throw new ArgumentOutOfRangeException("Message size cannot be less than 1 byte!");
        }

        collection["size"] = this.Size.ToString();
      }

      if (!checkStringIfNullEmptyWhitespace(this.AttachmentFileName))
      {
        collection["attachment"] = this.AttachmentFileName;
      }

      if (this.From != null)
      {
        collection["from"] = this.From.Address;
      }

      if (!checkStringIfNullEmptyWhitespace(this.Subject))
      {
        collection["subject"] = this.Subject;
      }

      if (this.SeverityType.HasValue)
      {
        collection["severity"] = getSeverityTypeName(this.SeverityType.Value);
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