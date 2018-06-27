using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Events
{
  public sealed class EventRequestBuilder : IEventRequestBuilder
  {
    private const int MAX_RESULT_LIMIT = 300;
    
    private IEventRequest eventRequest;

    public EventRequestBuilder()
    {
      this.eventRequest = new EventRequest();
    }

    public IEventRequestBuilder AddStartTimeRange(DateTime dateTime)
    {
      this.eventRequest.Begin = dateTime;

      return this;
    }

    public IEventRequestBuilder AddEndTimeRange(DateTime dateTime)
    {
      this.eventRequest.End = dateTime;

      return this;
    }

    public IEventRequestBuilder SortAscendingTimeRange(bool sortAscending)
    {
      this.eventRequest.Ascending = sortAscending;

      return this;
    }

    public IEventRequestBuilder AddResultLimit(int limit)
    {
      if (limit > MAX_RESULT_LIMIT)
      {
        throw new ArgumentOutOfRangeException("Limit of resulting events cannot exceed a maximum integer value of 300!");
      }

      this.eventRequest.Limit = limit;

      return this;
    }

    public IEventRequestBuilder AddMessageSize(int size)
    {
      if (size < 1)
      {
        throw new ArgumentOutOfRangeException("Message size cannot be less than 1 byte!");
      }

      this.eventRequest.Size = size;

      return this;
    }

    public IEventRequestBuilder AddAttachmentFilename(string name)
    {
      if (checkStringIfNullEmptyWhitespace(name))
      {
        throw new ArgumentNullException("Attachment Filename cannot be null or empty!");
      }

      this.eventRequest.AttachmentFileName = name;

      return this;
    }

    public IEventRequestBuilder AddRecipient(MailAddress address)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Recipient address cannot be null or empty!");
      }

      this.eventRequest.Recipient = address;

      return this;
    }

    public IEventRequestBuilder AddTo(MailAddress address)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Recipient address cannot be null or empty!");
      }

      this.eventRequest.To = address;

      return this;
    }

    public IEventRequestBuilder AddMessageId(string id)
    {
      if (checkStringIfNullEmptyWhitespace(id))
      {
        throw new ArgumentNullException("Message Id cannot be null or empty!");
      }

      this.eventRequest.MessageId = id;

      return this;
    }

    public IEventRequestBuilder AddSeverityType(Severity severityType)
    {
      this.eventRequest.SeverityType = severityType;

      return this;
    }

    public IEventRequestBuilder AddEventType(EventType eventType)
    {
      if (this.eventRequest.EventTypes == null)
      {
        this.eventRequest.EventTypes = new Collection<EventType>();
      }

      this.eventRequest.EventTypes.Add(eventType);

      return this;
    }

    public IEventRequestBuilder AddTagName(string tagName)
    {
      if (this.eventRequest.Tags == null)
      {
        this.eventRequest.Tags = new Collection<string>();
      }

      this.eventRequest.Tags.Add(tagName);

      return this;
    }

    public IEventRequestBuilder MakePretty(bool pretty)
    {
      this.eventRequest.Pretty = pretty;

      return this;
    }

    public IEventRequestBuilder AddFrom(MailAddress address)
    {
      if (address == null)
      {
        throw new ArgumentNullException("Address cannot be null or empty!");
      }

      this.eventRequest.From = address;

      return this;
    }

    public IEventRequest Build()
    {
      return this.eventRequest;
    }

    private bool checkStringIfNullEmptyWhitespace(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}