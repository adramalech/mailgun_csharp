using System;
using System.Text.RegularExpressions;
using MailgunSharp.Extensions;

namespace MailgunSharp.Routes
{
  public sealed class MatchHeader : IHeader
  {
    private readonly string name;
    public string Name { get; }

    private readonly Regex pattern;
    public Regex Pattern { get; }

    public MatchHeader(string name, Regex pattern)
    {
      if (name.IsNullEmptyWhitespace())
      {
        throw new ArgumentNullException("Header name cannot be null or empty!");
      }

      if (pattern == null)
      {
        throw new ArgumentNullException("Regular Expression pattern cannot be null or empty!");
      }

      this.name = name;
      this.pattern = pattern;
    }

    public override string ToString()
    {
      return $"match_header({this.name}, {this.pattern.ToString()})";
    }
  }
}