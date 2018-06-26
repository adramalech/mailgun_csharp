using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Events
{
  public sealed class EventRequest : IEventRequest
  {
    public DateTime? Begin { get; set; }
    public DateTime? End { get; set; }
    public bool? Ascending { get; set; }
    public int? Limit { get; set; }
    public ICollection<EventType> EventTypes { get; set; }
    public string AttachmentFileName { get; set; }
    public string MessageId { get; set; }
    public string Subject { get; set; }
    public string To { get; set; }
    public MailAddress Recipient { get; set; }
    public ICollection<string> Tags { get; set; }
    public Severity? Severity { get; set; }

    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      throw new NotImplementedException();
    }

    public JObject ToJson()
    {
      throw new NotImplementedException();
    }
  }
}