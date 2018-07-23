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

    public ICollection<string> Actions
    {
      get
      {
        return actions;
      }
    }

    private string expression;

    public string Expression
    {
      get
      {
        return expression;
      }
    }

    private string description;

    public string Description
    {
      get
      {
        return description;
      }
    }

    private int priority;

    public int Priority
    {
      get
      {
        return priority;
      }
    }

    public Route()
    {
      this.actions = new Collection<string>();
      this.expression = "";
      this.description = "";
      this.priority = 0;
    }

    public IRoute SetPriority(int priority)
    {
      if (priority < 0)
      {
        throw new ArgumentOutOfRangeException("Priority cannot have a value less than zero!");
      }

      this.priority = priority;

      return this;
    }

    public IRoute SetDescription(string description)
    {
      if (description.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Description cannot be null or empty!");
      }

      this.description = description;

      return this;
    }

    public IRoute SetFilter_MatchHeader(string name, Regex regex)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.expression = $"match_header({name}, {regex.ToString()})";

      return this;
    }

    public IRoute SetFilter_MatchHeader(string name, string value)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.expression = $"match_header({name}, {value})";

      return this;
    }

    public IRoute SetFilter_MatchRecipient(MailAddress emailAddress)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.expression = $"match_recipient({emailAddress.Address})";

      return this;
    }

    public IRoute SetFilter_MatchRecipient(Regex regex)
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.actions.Add($"match_recipient({regex.ToString()})");

      return this;
    }

    public IRoute SetFilter_CatchAll()
    {
      if (!this.expression.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Expression can only be set once!");
      }

      this.actions.Add("catch_all()");

      return this;
    }

    public IRoute AddAction_Forward(Uri uri)
    {
      this.actions.Add($"forward({uri.ToString()})");

      return this;
    }

    public IRoute AddAction_Forward(MailAddress emailAddress)
    {
      this.actions.Add($"forward({emailAddress.Address})");

      return this;
    }

    public IRoute AddAction_Store(Uri uri)
    {
      this.actions.Add($"forward({uri.ToString()})");

      return this;
    }

    public IRoute AddAction_Stop()
    {
      this.actions.Add("stop()");

      return this;
    }


  }
}