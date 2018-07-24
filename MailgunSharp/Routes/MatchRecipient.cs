using System;
using System.Text.RegularExpressions;

namespace MailgunSharp.Routes
{
  public sealed class MatchRecipient : IPattern
  {
    private readonly Regex pattern;

    public Regex Pattern { get; }

    public MatchRecipient(Regex pattern)
    {
      if (pattern == null)
      {
        throw new ArgumentNullException("Pattern cannot be null or empty!");
      }

      this.pattern = pattern;
    }

    public override string ToString()
    {
      return $"match_recipient({this.pattern.ToString()})";
    }
  }
}