using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MailgunSharp.Routes
{
  public interface IRoute
  {
    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    string Description { get; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    int Priority { get; }

    /// <summary>
    ///
    /// </summary>
    /// <value></value>
    string Expression { get; }

    /// <summary>
    /// If a route expression evaluates to true, Mailgun executes the corresponding action.
    /// Currently you can use the following three actions in your routes: "forward", "store", and/or "stop".
    /// </summary>
    /// <value></value>
    ICollection<string> Actions { get; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
    IRoute SetPriority(int priority);

    /// <summary>
    ///
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    IRoute SetDescription(string description);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    IRoute SetFilter_MatchHeader(string name, Regex regex);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRoute SetFilter_MatchHeader(string name, string value);

    /// <summary>
    ///
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    IRoute SetFilter_MatchRecipient(MailAddress emailAddress);

    /// <summary>
    ///
    /// </summary>
    /// <param name="regex"></param>
    /// <returns></returns>
    IRoute SetFilter_MatchRecipient(Regex regex);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IRoute SetFilter_CatchAll();

    /// <summary>
    ///
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IRoute AddAction_Forward(Uri uri);

    /// <summary>
    ///
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    IRoute AddAction_Forward(MailAddress emailAddress);

    /// <summary>
    ///
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IRoute AddAction_Store(Uri uri);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IRoute AddAction_Stop();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    ICollection<KeyValuePair<string, string>> AsFormContent();
  }
}