namespace MailgunSharp
{
  public interface IQueryString
  {
    int Count { get; }
    bool AppendIfNotNullOrEmpty<T>(string var, T val) where T : struct;
    bool AppendIfNotNullOrEmpty(string var, string val);
  }
}
