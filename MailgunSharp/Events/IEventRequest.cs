using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Events
{
  public interface IEventRequest
  {
    DateTime? Begin { get; set; }
    DateTime? End { get; set; }
    bool? Ascending { get; set; }
    int? Limit { get; set; }
    int? Size { get; set; }
    ICollection<EventType> EventTypes { get; set; }
    string AttachmentFileName { get; set; }
    string MessageId { get; set; }
    string Subject { get; set; }
    MailAddress To { get; set; }
    MailAddress Recipient { get; set; }
    ICollection<string> Tags { get; set; }
    Severity? Severity { get; set; }

    string ToQueryString();
  }
}