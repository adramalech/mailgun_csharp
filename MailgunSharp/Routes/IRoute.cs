using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MailgunSharp.Routes
{
  public interface IRoute
  {
    /// <summary>
    /// A description of the route.
    /// </summary>
    /// <value>string</value>
    string Description { get; }

    /// <summary>
    /// Smaller number indicates higher priority. Higher priority routes are handled first.
    /// </summary>
    /// <value>int</value>
    int Priority { get; }

    /// <summary>
    /// Route filter expressions that determine when an action is triggered.
    /// You can create a filter based on the recipient of the incoming email,
    /// the headers in the incoming email or use a catch-all filter.
    /// Filters support regular expressions in the pattern.
    /// </summary>
    /// <value>string</value>
    string Expression { get; }

    /// <summary>
    /// If a route expression evaluates to true, Mailgun executes the corresponding action.
    /// Currently you can use the following three actions in your routes: "forward", "store", and/or "stop".
    /// </summary>
    /// <value>A list of actions the route will take.</value>
    ICollection<string> Actions { get; }

    /// <summary>
    /// Sets the priority of the route 0 is highest and the bigger the number the lower the priority.
    /// </summary>
    /// <param name="priority">The priority value.</param>
    /// <returns>An instance of the route.</returns>
    IRoute SetPriority(int priority);

    /// <summary>
    /// Sets the description of the route.
    /// </summary>
    /// <param name="description">The description of the route.</param>
    /// <returns>An instance of the route.</returns>
    IRoute SetDescription(string description);

    /// <summary>
    /// Matches arbitrary MIME header of the message against the regular expression pattern.
    /// </summary>
    /// <param name="name">The name of the MIME header.</param>
    /// <param name="value">The regular expression pattern.</param>
    /// <returns>An instance of the route.</returns>
    IRoute MatchHeader(string name, Regex regex);

    /// <summary>
    /// Matches SMTP recipient of the incoming message against the regular expression pattern.
    /// </summary>
    /// <param name="regex">The regular expression pattern to match.</param>
    /// <returns>An instance of the route.</returns>
    IRoute MatchRecipient(Regex regex);

    /// <summary>
    /// Matches if no preceeding routes matched. Usually you would use this
    /// in a route with the lowest priority, to make sure it evaluates last.
    /// </summary>
    /// <returns>An instance of the route.</returns>
    IRoute CatchAll();

    /// <summary>
    /// Forwards the message to a specified destination URL.
    /// </summary>
    /// <param name="emailAddress">The URL address to forward to.</param>
    /// <returns>An instance of the route.</returns>
    IRoute Forward(Uri uri);

    /// <summary>
    /// Forwards the message to a specified destination email address.
    /// </summary>
    /// <param name="emailAddress">The email address to forward to.</param>
    /// <returns>An instance of the route.</returns>
    IRoute Forward(MailAddress emailAddress);

    /// <summary>
    /// Stores the message temporarily (for up to 3 days) on Maigun's server
    /// so you can retrieve them later.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>An instance of the route.</returns>
    IRoute Store(Uri uri);

    /// <summary>
    /// Simply stops the priority waterfall so the subsequent routes will
    /// not be evaluated.
    /// </summary>
    /// <returns>An instance of the route.</returns>
    IRoute Stop();

    /// <summary>
    /// Get the Route object as a form content to submit in an http request.
    /// </summary>
    /// <returns>The form content as a keyvalue string pairs.</returns>
    ICollection<KeyValuePair<string, string>> AsFormContent();
  }
}