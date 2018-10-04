using NodaTime;
using System.Net.Mail;
using MailgunSharp.Enums;

namespace MailgunSharp.Events
{
  public interface IEventRequestBuilder
  {
    /// <summary>
    /// Set the beginning search time range to filter results by.
    /// </summary>
    /// <param name="dateTime">The beginning time range value.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetStartTimeRange(Instant dateTime);

    /// <summary>
    /// Set the end search time range to filter results by.
    /// </summary>
    /// <param name="dateTime">The end time range value.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetEndTimeRange(Instant dateTime);

    /// <summary>
    /// Set the direction of the search time range.
    /// </summary>
    /// <param name="sortAscending"></param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetSortByAscendingTimeRange(bool sortAscending);

    /// <summary>
    /// Set the result entries limit.
    /// </summary>
    /// <param name="limit">Integer value to limit entires by.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetResultLimit(int limit);

    /// <summary>
    /// Set the size of the message represented in bytes to filter results by.
    /// </summary>
    /// <param name="size">Integer value representing the size in bytes of a message.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetMessageSize(int size);

    /// <summary>
    /// Set the attachment filename to filter results by.
    /// </summary>
    /// <param name="name">The name of the file.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetAttachmentFilename(string name);

    /// <summary>
    /// Set the Mailgun message id returned by the messages API in the receipt of sending.
    /// </summary>
    /// <param name="id">The string message id.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetMessageId(string id);

    /// <summary>
    /// Set the severity to filter by.
    /// </summary>
    /// <param name="severity">The severity to filter by.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetSeverityType(Severity severity);

    /// <summary>
    /// Add an event type to the list of event types to filter results by.
    /// </summary>
    /// <param name="eventType">A event type to add to the list of event types.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder AddEventType(EventType eventType);

    /// <summary>
    /// Add a user-defined tag name to filter results to a list of tags to filter.
    /// </summary>
    /// <param name="tagName">The tag to add to list of tags to filter by.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder AddTagName(string tagName);

    /// <summary>
    /// Make the response json object formatted pretty.
    /// </summary>
    /// <param name="pretty">Pretty flag true will return pretty formatted result, false will return normal.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder MakePretty(bool pretty);

    /// <summary>
    /// Filter the "from" MIME header.
    /// </summary>
    /// <param name="address">The valid email address to filter "from" MIME header.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetFrom(MailAddress address);

    /// <summary>
    /// Filter the email address of a particular recipient.
    /// </summary>
    /// <param name="address">A valid email address of a recipient that was sent to.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetRecipient(MailAddress address);

    /// <summary>
    /// Filter the "to" MIME header.
    /// </summary>
    /// <param name="address">A valid email address found in the "to" MIME header.</param>
    /// <returns>The instance of the builder.</returns>
    IEventRequestBuilder SetTo(MailAddress address);

    /// <summary>
    /// Get the event request that was build.
    /// </summary>
    /// <returns>Event Request</returns>
    IEventRequest Build();
  }
}
