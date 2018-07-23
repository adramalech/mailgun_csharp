using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using MailgunSharp.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MailgunSharp.Routes
{
  public sealed class Route : IRoute
  {
    private ICollection<string> actions;

    /// <summary>
    /// If a route expression evaluates to true, Mailgun executes the corresponding action.
    /// Currently you can use the following three actions in your routes: "forward", "store", and/or "stop".
    /// </summary>
    /// <value>A list of actions the route will take.</value>
    public ICollection<string> Actions
    {
      get
      {
        return actions;
      }
    }

    private string expression;

    /// <summary>
    /// Route filter expressions that determine when an action is triggered.
    /// You can create a filter based on the recipient of the incoming email,
    /// the headers in the incoming email or use a catch-all filter.
    /// Filters support regular expressions in the pattern.
    /// </summary>
    /// <value>string</value>
    public string Expression
    {
      get
      {
        return expression;
      }
    }

    private string description;

    /// <summary>
    /// A description of the route.
    /// </summary>
    /// <value>string</value>
    public string Description
    {
      get
      {
        return description;
      }
    }

    private int priority;

    /// <summary>
    /// Smaller number indicates higher priority. Higher priority routes are handled first.
    /// </summary>
    /// <value>int</value>
    public int Priority
    {
      get
      {
        return priority;
      }
    }

    /// <summary>
    /// Create a new instance of the route class.
    /// </summary>
    public Route()
    {
      this.actions = new Collection<string>();
      this.expression = "";
      this.description = "";
      this.priority = 0;
    }

    /// <summary>
    /// Sets the priority of the route 0 is highest and the bigger the number the lower the priority.
    /// </summary>
    /// <param name="priority">The priority value.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute SetPriority(int priority)
    {
      if (priority < 0)
      {
        throw new ArgumentOutOfRangeException("Priority cannot have a value less than zero!");
      }

      this.priority = priority;

      return this;
    }

    /// <summary>
    /// Sets the description of the route.
    /// </summary>
    /// <param name="description">The description of the route.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute SetDescription(string description)
    {
      if (description.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Description cannot be null or empty!");
      }

      this.description = description;

      return this;
    }

    /// <summary>
    /// Matches arbitrary MIME header of the message against the regular expression pattern.
    /// </summary>
    /// <param name="name">The name of the MIME header.</param>
    /// <param name="value">The regular expression pattern.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute MatchHeader(string name, Regex regex)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.expression = $"match_header({name}, {regex.ToString()})";

      return this;
    }

    /// <summary>
    /// Matches SMTP recipient of the incoming message against the regular expression pattern.
    /// </summary>
    /// <param name="regex">The regular expression pattern to match.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute MatchRecipient(Regex regex)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.actions.Add($"match_recipient({regex.ToString()})");

      return this;
    }

    /// <summary>
    /// Matches if no preceeding routes matched. Usually you would use this
    /// in a route with the lowest priority, to make sure it evaluates last.
    /// </summary>
    /// <returns>An instance of the route.</returns>
    public IRoute CatchAll()
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.actions.Add("catch_all()");

      return this;
    }

    /// <summary>
    /// Forwards the message to a specified destination URL.
    /// </summary>
    /// <param name="emailAddress">The URL address to forward to.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute Forward(Uri uri)
    {
      this.actions.Add($"forward({uri.ToString()})");

      return this;
    }

    /// <summary>
    /// Forwards the message to a specified destination email address.
    /// </summary>
    /// <param name="emailAddress">The email address to forward to.</param>
    /// <returns>An instance of the route.</returns>
    public IRoute Forward(MailAddress emailAddress)
    {
      this.actions.Add($"forward({emailAddress.Address})");

      return this;
    }

    /// <summary>
    /// Stores the message temporarily (for up to 3 days) on Maigun's server
    /// so you can retrieve them later.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>An instance of the route.</returns>
    public IRoute Store(Uri uri)
    {
      this.actions.Add($"forward({uri.ToString()})");

      return this;
    }

    /// <summary>
    /// Simply stops the priority waterfall so the subsequent routes will
    /// not be evaluated.
    /// </summary>
    /// <returns>An instance of the route.</returns>
    public IRoute Stop()
    {
      this.actions.Add("stop()");

      return this;
    }

    /// <summary>
    /// Get the Route object as a form content to submit in an http request.
    /// </summary>
    /// <returns>The form content as a keyvalue string pairs.</returns>
    public ICollection<KeyValuePair<string, string>> AsFormContent()
    {
      if (this.actions == null || this.actions.Count < 1)
      {
        throw new ArgumentNullException("Unable to create route without any actions!");
      }

      if (this.expression.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Unable to create route without an expression!");
      }

      var formContent = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("priority", this.priority.ToString()),
        new KeyValuePair<string, string>("description", this.description),
        new KeyValuePair<string, string>("expression", this.expression)
      };

      foreach(var action in this.actions)
      {
        formContent.Add("action", action);
      }

      return formContent;
    }
  }
}