using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Enums;
using MailgunSharp.Extensions;

namespace MailgunSharp.Webhooks
{
  public sealed class Webhook : IWebhook
  {
    /// <summary>
    /// The maximum allowed number of urls to be attached to the webhook.
    /// </summary>
    private const int MAX_URL_COUNT = 3;

    private WebHookType type;

    /// <summary>
    /// Get the webhook type assigned to this webhook.
    /// </summary>
    /// <value>WebhookType enum.</value>
    public WebHookType Type => this.type;

    private string id;

    /// <summary>
    /// The type id of the webhook.
    /// </summary>
    /// <value>Type id of the webhook.</value>
    public string Id => this.id;

    private ICollection<Uri> urls;

    /// <summary>
    /// Up to three urls can be added to the webhook.
    /// </summary>
    /// <value>The three urls that are attached to the webhook.</value>
    public ICollection<Uri> Urls => this.urls;

    /// <summary>
    /// Create an instance of the webhook.
    /// </summary>
    public Webhook()
    {
      this.urls = new Collection<Uri>();
    }

    /// <summary>
    /// Set the type id of the webhook.
    /// </summary>
    /// <param name="type">The type of webhook.</param>
    /// <returns>The instance of the webhook.</returns>
    public IWebhook SetTypeId(WebHookType type)
    {
      this.type = type;
      this.id = EnumLookup.GetWebhookTypeName(type);

      return this;
    }

    /// <summary>
    /// Add a webhook to the list of urls, maximum of three urls allowed, to the webhook.
    /// </summary>
    /// <param name="uri">The url to be added to the webhook.</param>
    /// <returns>The instance of the webhook.</returns>
    public IWebhook AppendUrl(Uri uri)
    {
      if (this.urls.Count >= MAX_URL_COUNT)
      {
        throw new InvalidOperationException("The webhook cannot have more than a maximum of three urls!");
      }

      this.urls.Add(uri);

      return this;
    }

    /// <summary>
    /// Get the webhook as form content to be used in http requests.
    /// </summary>
    /// <returns>A keyvalue string pair representation of the webhook.</returns>
    public ICollection<KeyValuePair<string, string>> ToFormContent()
    {
      if (this.id.IsNullEmptyWhitespace())
      {
        throw new InvalidOperationException("Cannot create a webhook form content without a type!");
      }

      if (this.urls.Count < 1)
      {
        throw new InvalidOperationException("Cannot create a webhook form content without atleast one url!");
      }

      var content = new Collection<KeyValuePair<string, string>>();

      content.Add("id", this.id);

      foreach (var url in this.urls)
      {
        content.Add("url", url.ToString());
      }

      return content;
    }
  }
}