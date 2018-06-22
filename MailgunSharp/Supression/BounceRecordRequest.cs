namespace MailgunSharp.Supression
{
  public class BounceRecordRequest
  {
    public string address { get; set; }
    public int code { get; set; }
    public string error { get; set; }
    public long created_at { get; set; }
  }
}
