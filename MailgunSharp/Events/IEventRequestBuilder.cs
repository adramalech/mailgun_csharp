using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Events
{
  public interface IEventRequestBuilder
  {
    IEventRequestBuilder AddStartTimeRange(DateTime dateTime);
    IEventRequestBuilder AddEndTimeRange(DateTime dateTime);
    IEventRequestBuilder SortAscendingTimeRange(bool sortAscending);
    IEventRequestBuilder AddResultLimit(int limit);
    IEventRequestBuilder AddAttachmentFilename(string name);
    IEventRequestBuilder AddMessageId(string id);
    IEventRequestBuilder AddSeverity(Severity severity);
    IEventRequestBuilder AddEventType(EventType eventType);
    IEventRequestBuilder AddTagName(string tagName);

    IEventRequest Build();
  }
}