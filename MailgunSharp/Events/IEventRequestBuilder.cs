using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Events
{
  public interface IEventRequestBuilder
  {
    IEventRequestBuilder SetStartTimeRange(DateTime dateTime);
    IEventRequestBuilder SetEndTimeRange(DateTime dateTime);
    IEventRequestBuilder SetSortByAscendingTimeRange(bool sortAscending);
    IEventRequestBuilder SetResultLimit(int limit);
    IEventRequestBuilder SetMessageSize(int size);
    IEventRequestBuilder SetAttachmentFilename(string name);
    IEventRequestBuilder SetMessageId(string id);
    IEventRequestBuilder SetSeverityType(Severity severity);
    IEventRequestBuilder AddEventType(EventType eventType);
    IEventRequestBuilder AddTagName(string tagName);
    IEventRequestBuilder MakePretty(bool pretty);
    IEventRequestBuilder SetFrom(MailAddress address);

    IEventRequest Build();
  }
}
