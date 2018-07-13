using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using MailgunSharp.Enums;

namespace MailgunSharp.Events
{
  public interface IEventRequest
  {
    /// <summary>
    /// The beginning of the search time range.
    /// </summary>
    /// <value>DateTime</value>
    DateTime? Begin { get; set; }

    /// <summary>
    /// The end of the search time range.
    /// </summary>
    /// <value>DateTime</value>
    DateTime? End { get; set; }

    /// <summary>
    /// Defines the direction of the search time range and must be provided if the range end time is not specified.
    /// </summary>
    /// <value>boolean</value>
    bool? Ascending { get; set; }

    /// <summary>
    /// Return the response in pretty json.
    /// </summary>
    /// <value>boolean</value>
    bool? Pretty { get; set; }

    /// <summary>
    /// The number of entries to return in the response.
    /// </summary>
    /// <value>Integer</value>
    int Limit { get; set; }

    /// <summary>
    /// Message size as an int value representing number of bytes.
    /// </summary>
    /// <value>Integer</value>
    int? Size { get; set; }

    /// <summary>
    /// The list of events to filter by.
    /// </summary>
    /// <value>List of event types.</value>
    ICollection<EventType> EventTypes { get; set; }

    /// <summary>
    /// The attachment filename to search for.
    /// </summary>
    /// <value>string</value>
    string AttachmentFileName { get; set; }

    /// <summary>
    /// A Mailgun message id returned by the messages API to search for.
    /// </summary>
    /// <value>string</value>
    string MessageId { get; set; }

    /// <summary>
    /// A subject line to filter by.
    /// </summary>
    /// <value>string</value>
    string Subject { get; set; }

    /// <summary>
    /// An email address mentioned in the "to" MIME header.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress To { get; set; }

    /// <summary>
    /// An email address mentioned in the "from" MIME header.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress From { get; set; }

    /// <summary>
    /// An email address of a particular recipient.
    /// </summary>
    /// <value>System.Net.Mail.MailAddress</value>
    MailAddress Recipient { get; set; }

    /// <summary>
    /// User defined tags to filter by.
    /// </summary>
    /// <value>List of strings</value>
    ICollection<string> Tags { get; set; }

    /// <summary>
    /// Used to filter events based on severity, if exists. (Currently failed events only.)
    /// </summary>
    /// <value>Severity type.</value>
    Severity? SeverityType { get; set; }

    /// <summary>
    /// Get the event request object as a query string to be used in an http request.
    /// </summary>
    /// <returns>string</returns>
    string ToQueryString();
  }
}