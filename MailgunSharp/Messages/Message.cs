using System;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public sealed class Message : IMessage
  {
    public MailAddress From { get; set; }
    public ICollection<IRecipient> To { get; set; }
    public ICollection<MailAddress> Cc { get; set; }
    public ICollection<MailAddress> Bcc { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public string Html { get; set; }
    public ICollection<IFileAttachment> FileAttachments { get; set; }
    public ICollection<FileInfo> Attachments { get; set; }
    public ICollection<IFileAttachment> FileInline { get; set; }
    public ICollection<FileInfo> Inline { get; set; }
    public ICollection<string> Tags { get; set; }
    public string CampaignId { get; set; }
    public bool Dkim { get; set; }
    public DateTime? DeliveryTime { get; set; }
    public bool TestMode { get; set; }
    public bool Tracking { get; set; }
    public bool TrackingClicks { get; set; }
    public bool TrackingOpens { get; set; }
    public JObject RecipientVariables { get; set; }
    public IDictionary<string, string> CustomHeaders { get; set; }
    public IDictionary<string, JObject> CustomData { get; set; }
    public bool SkipTlsVerification { get; set; }
    public bool SendSecure { get; set; }

    private const long MAX_TOTAL_MESSAGE_SIZE = 25000000;
    private const int MAX_RECIPIENT_SIZE = 1000;

    public HttpContent AsFormContent()
    {
      var formContent = new MultipartFormDataContent();

      if (this.Attachments != null)
      {
        foreach (var attachment in this.Attachments)
        {
          formContent.Add(new ByteArrayContent(File.ReadAllBytes(attachment.FullName)), "attachment", attachment.Name);
        }
      }

      if (this.FileAttachments != null)
      {
        foreach (var attachment in this.FileAttachments)
        {
          formContent.Add(new ByteArrayContent(attachment.Data), "attachment", attachment.FileName);
        }
      }

      if (this.Inline != null)
      {
        foreach (var image in this.Inline)
        {
          formContent.Add(new StreamContent(image.OpenRead()), "inline", image.Name);
        }
      }

      if (this.FileInline != null)
      {
        foreach (var image in this.FileInline)
        {
          formContent.Add(new ByteArrayContent(image.Data), "inline", image.FileName);
        }
      }

      foreach(var content in AsKeyValueCollection())
      {
        formContent.Add(new StringContent(content.Value), content.Key);
      }

      return formContent;
    }

    public ICollection<KeyValuePair<string, string>> AsKeyValueCollection()
    {
      if (this.To == null)
      {
        throw new ArgumentNullException("Recipients cannot be empty!");
      }

      if (this.To.Count > MAX_RECIPIENT_SIZE)
      {
        throw new ArgumentOutOfRangeException("Maximum number of 1,000 recipients cannot be exceeded!");
      }

      if (this.RecipientVariables != null && this.RecipientVariables.Count != this.To.Count)
      {
        throw new ArgumentOutOfRangeException("Did not have matching amount of recipient variables and recipients!");
      }

      if (this.From == null)
      {
        throw new ArgumentNullException("Sender cannot be null!");
      }

      var content = new Collection<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("from", this.From.Address.ToString()),
        new KeyValuePair<string, string>("o:testmode", boolAsYesNo(this.TestMode)),
        new KeyValuePair<string, string>("o:tracking", boolAsYesNo(this.Tracking)),
        new KeyValuePair<string, string>("o:tracking-clicks", boolAsYesNo(this.TrackingClicks)),
        new KeyValuePair<string, string>("o:tracking-opens", boolAsYesNo(this.TrackingOpens)),
        new KeyValuePair<string, string>("o:require-tls", this.SendSecure.ToString()),
        new KeyValuePair<string, string>("o:skip-verfication", this.SkipTlsVerification.ToString()),
        new KeyValuePair<string, string>("o:dkim", boolAsYesNo(this.Dkim))
      };

      var addressList = new Collection<MailAddress>();

      foreach (var t in this.To)
      {
        addressList.Add(t.Address);
      }

      content.Add(new KeyValuePair<string, string>("to", generateCommaDelimenatedList(addressList)));

      if (this.Cc != null)
      {
        content.Add(new KeyValuePair<string, string>("cc", generateCommaDelimenatedList(this.Cc)));
      }

      if (this.Bcc != null)
      {
        content.Add(new KeyValuePair<string, string>("bcc", generateCommaDelimenatedList(this.Bcc)));
      }

      if (!isStringNullOrEmpty(this.Subject))
      {
        content.Add(new KeyValuePair<string, string>("subject", this.Subject));
      }

      if (!isStringNullOrEmpty(this.Html))
      {
        content.Add(new KeyValuePair<string, string>("html", this.Html));
      }

      if (!isStringNullOrEmpty(this.Text))
      {
        content.Add(new KeyValuePair<string, string>("text", this.Text));
      }

      if (!isStringNullOrEmpty(this.CampaignId))
      {
        content.Add(new KeyValuePair<string, string>("o:campaign", this.CampaignId));
      }

      if (this.RecipientVariables != null)
      {
        content.Add(new KeyValuePair<string, string>("recipient-variables", this.RecipientVariables.ToString(Formatting.None)));
      }

      if (this.Tags != null)
      {
        foreach (var tag in this.Tags)
        {
          content.Add(new KeyValuePair<string, string>("o:tag", tag));
        }
      }

      if (this.CustomHeaders != null)
      {
        foreach (var customHeader in this.CustomHeaders)
        {
          content.Add(new KeyValuePair<string, string>($"h:{customHeader.Key}", customHeader.Value));
        }
      }

      if (this.CustomData != null)
      {
        foreach (var data in this.CustomData)
        {
          content.Add(new KeyValuePair<string, string>($"v:{data.Key}", data.Value.ToString(Formatting.None)));
        }
      }

      if (this.DeliveryTime != null && this.DeliveryTime.HasValue)
      {
        content.Add(new KeyValuePair<string, string>("o:deliverytime", ((DateTimeOffset)this.DeliveryTime.Value).ToUnixTimeSeconds().ToString()));
      }

      return content;
    }

    private string boolAsYesNo(bool flag)
    {
      return (flag ? "yes" : "no");
    }

    private bool isStringNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }

    private string generateCommaDelimenatedList(ICollection<MailAddress> addresses)
    {
      var addressList = new StringBuilder();

      var i = 1;
      var total = addresses.Count;

      var str = "";

      foreach (var address in addresses)
      {
        str = address.ToString();

        if (i < total)
        {
          str += ",";
        }

        addressList.Append(str);
        i++;
      }

      return addressList.ToString();
    }
  }
}
