using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public interface IMessage
  {
    IRecipient From { get; set; }
    ICollection<IRecipient> To { get; set; }
    ICollection<IRecipient> Cc { get; set; }
    ICollection<IRecipient> Bcc { get; set; }
    string Subject { get; set; }
    string Text { get; set; }
    string Html { get; set; }
    ICollection<IFileAttachment> FileAttachments { get; set; }
    ICollection<FileInfo> Attachments { get; set; }
    ICollection<IFileAttachment> FileInline { get; set; }
    ICollection<FileInfo> Inline { get; set; }
    ICollection<string> Tags { get; set; }
    string CampaignId { get; set; }
    bool Dkim { get; set; }
    DateTime? DeliveryTime { get; set; }
    bool TestMode { get; set; }
    bool Tracking { get; set; }
    bool TrackingClicks { get; set; }
    bool TrackingOpens { get; set; }
    bool SkipTlsVerification { get; set; }
    bool SendSecure { get; set; }
    JObject RecipientVariables { get; set; }
    IDictionary<string, string> CustomHeaders { get; set; }
    IDictionary<string, JObject> CustomData { get; set; }
    ICollection<KeyValuePair<string, string>> AsKeyValueCollection();
    HttpContent AsFormContent();
  }
}
