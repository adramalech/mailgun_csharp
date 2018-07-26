using System;
using System.Collections.Generic;
using MailgunSharp.Enums;

namespace MailgunSharp.Webhooks
{
  public interface IWebhook
  {
    /// <summary>
    /// Get the webhook type assigned to this webhook.
    /// </summary>
    /// <value>WebhookType enum.</value>
    WebHookType Type { get; }

    /// <summary>
    /// The type id of the webhook.
    /// </summary>
    /// <value>Type id of the webhook.</value>
    string Id { get; }

    /// <summary>
    /// Up to three urls can be added to the webhook.
    /// </summary>
    /// <value>The three urls that are attached to the webhook.</value>
    ICollection<Uri> Urls { get; }

    /// <summary>
    /// Set the type id of the webhook.
    /// </summary>
    /// <param name="type">The type of webhook.</param>
    /// <returns>The instance of the webhook.</returns>
    IWebhook SetTypeId(WebHookType type);

    /// <summary>
    /// Add a webhook to the list of urls, maximum of three urls allowed, to the webhook.
    /// </summary>
    /// <param name="uri">The url to be added to the webhook.</param>
    /// <returns>The instance of the webhook.</returns>
    IWebhook AppendUrl(Uri uri);

    /// <summary>
    /// Get the webhook as form content to be used in http requests.
    /// </summary>
    /// <returns>A keyvalue string pair representation of the webhook.</returns>
    ICollection<KeyValuePair<string, string>> ToFormContent();
  }
}