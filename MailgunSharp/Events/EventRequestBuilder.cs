using System;
using System.Net.Mail;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;
using NodaTime;

namespace MailgunSharp.Events
{
  public sealed class EventRequestBuilder : IEventRequestBuilder
  {
    /// <summary>
    /// Maximum number of entries to return in the response.
    /// </summary>
    private const int MAX_RESULT_LIMIT = 300;

    /// <summary>
    /// Instance of event request to create.
    /// </summary>
    private IEventRequest eventRequest;

    /// <summary>
    /// Create an instance of the event request builder class.
    ///
    /// Initialize the event request to build.
    /// </summary>
    public EventRequestBuilder()
    {
      this.eventRequest = new EventRequest();
    }

    /// <summary>
    /// Set the beginning search time range to filter results by.
    /// </summary>
    /// <param name="dateTime">The beginning time range value.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetStartTimeRange(Instant dateTime)
    {
      this.eventRequest.Begin = dateTime;

      return this;
    }

    /// <summary>
    /// Set the end search time range to filter results by.
    /// </summary>
    /// <param name="dateTime">The end time range value.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetEndTimeRange(Instant dateTime)
    {
      this.eventRequest.End = dateTime;

      return this;
    }

    /// <summary>
    /// Set the direction of the search time range.
    ///
    /// Disclaimer must be provided if the range end time is not specified.
    /// </summary>
    /// <param name="sortAscending"></param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetSortByAscendingTimeRange(bool sortAscending)
    {
      this.eventRequest.Ascending = sortAscending;

      return this;
    }

    /// <summary>
    /// Set the result entries limit.
    ///
    /// The maximum limit cannot be greater than 300 and must be greater than 0.
    /// </summary>
    /// <param name="limit">Integer value to limit entires by, must be a postive integer value greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetResultLimit(int limit)
    {
      if (limit > MAX_RESULT_LIMIT || limit < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(limit), limit, "Limit of resulting events cannot exceed a maximum integer value of 300, or the minimum of 1!");
      }

      this.eventRequest.Limit = limit;

      return this;
    }

    /// <summary>
    /// Set the size of the message represented in bytes to filter results by.
    /// </summary>
    /// <param name="size">Integer value representing the size in bytes of a message. Must be a positive integer value greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Message size must be an integer value greater than 0 bytes.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetMessageSize(int size)
    {
      if (size < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(size), size, "Message size cannot be less than 1 byte!");
      }

      this.eventRequest.Size = size;

      return this;
    }

    /// <summary>
    /// Set the attachment filename to filter results by.
    /// </summary>
    /// <param name="name">The name of the file.</param>
    /// <exception cref="ArgumentNullException">The filename must not be empty, null, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetAttachmentFilename(string name)
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(name), "Attachment Filename cannot be null or empty!");
      }

      this.eventRequest.AttachmentFileName = name;

      return this;
    }

    /// <summary>
    /// Set the Mailgun message id returned by the messages API in the receipt of sending.
    /// </summary>
    /// <param name="id">The string message id.</param>
    /// <exception cref="ArgumentNullException">The message id must not be empty, null, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetMessageId(string id)
    {
      if (id.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException(nameof(id), "Message Id cannot be null or empty!");
      }

      this.eventRequest.MessageId = id;

      return this;
    }

    /// <summary>
    /// Set the severity to filter by.
    /// Disclaimer: Will only work if the event type "failed" has been added.
    /// </summary>
    /// <param name="severityType">The severity to filter by.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetSeverityType(Severity severityType)
    {
      this.eventRequest.SeverityType = severityType;

      return this;
    }

    /// <summary>
    /// Add an event type to the list of event types to filter results by.
    /// </summary>
    /// <param name="eventType">A event type to add to the list of event types.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder AddEventType(EventType eventType)
    {
      if (this.eventRequest.EventTypes == null)
      {
        this.eventRequest.EventTypes = new Collection<EventType>();
      }

      this.eventRequest.EventTypes.Add(eventType);

      return this;
    }

    /// <summary>
    /// Add a user-defined tag name to filter results to a list of tags to filter.
    /// </summary>
    /// <param name="tagName">The tag to add to list of tags to filter by.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder AddTagName(string tagName)
    {
      if (this.eventRequest.Tags == null)
      {
        this.eventRequest.Tags = new Collection<string>();
      }

      this.eventRequest.Tags.Add(tagName);

      return this;
    }

    /// <summary>
    /// Make the response json object formatted pretty.
    /// </summary>
    /// <param name="pretty">Pretty flag true will return pretty formatted result, false will return normal.</param>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder MakePretty(bool pretty)
    {
      this.eventRequest.Pretty = pretty;

      return this;
    }

    /// <summary>
    /// Filter the "from" MIME header by.
    /// </summary>
    /// <param name="address">The valid email address to filter "from" MIME header by.</param>
    /// <exception cref="ArgumentNullException">Address cannot be null, empty, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetFrom(MailAddress address)
    {
      this.eventRequest.From = address ?? throw new ArgumentNullException(nameof(address), "Address cannot be null or empty!");

      return this;
    }

    /// <summary>
    /// Filter the email address of a particular recipient.
    /// </summary>
    /// <param name="address">A valid email address of a recipient that was sent to.</param>
    /// <exception cref="ArgumentNullException">Address cannot be null, empty, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetRecipient(MailAddress address)
    {
      this.eventRequest.Recipient = address ?? throw new ArgumentNullException(nameof(address), "Recipient address cannot be null or empty!");

      return this;
    }

    /// <summary>
    /// Filter the "to" MIME header.
    /// </summary>
    /// <param name="address">A valid email address found in the "to" MIME header.</param>
    /// <exception cref="ArgumentNullException">Address cannot be null, empty, or whitespace.</exception>
    /// <returns>The instance of the builder.</returns>
    public IEventRequestBuilder SetTo(MailAddress address)
    {
      this.eventRequest.To = address ?? throw new ArgumentNullException(nameof(address), "Recipient address cannot be null or empty!");

      return this;
    }

    /// <summary>
    /// Get the event request that was build.
    /// </summary>
    /// <returns>Event Request</returns>
    public IEventRequest Build() => this.eventRequest;
  }
}
