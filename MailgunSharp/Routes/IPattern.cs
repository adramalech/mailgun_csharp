using System.Text.RegularExpressions;

namespace MailgunSharp.Routes
{
  public interface IPattern
  {
    Regex Pattern { get; }
  }
}