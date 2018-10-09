using System;
using Xunit;
using MailgunSharp.Webhooks;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Webhooks
{
  public class WebhookTest
  {
    [Fact]
    public void Initialize_Webhook_Should_Have_Initialized_Collection_Of_URLs()
    {
      var webhook = new Webhook();

      var urls = webhook.Urls;

      Assert.Empty(urls);
    }

    [Fact]
    public void Webhook_Should_Throw_Error_If_More_Than_Three_URLs_Appended()
    {
      var webhook = new Webhook();

      Assert.Throws<InvalidOperationException>(() => {
        webhook.AppendUrl(new Uri(@"https://example1.sample.com"))
               .AppendUrl(new Uri(@"https://example2.sample.com"))
               .AppendUrl(new Uri(@"https://example3.sample.com"))
               .AppendUrl(new Uri(@"https://example4.sample.com"));
      });
    }

    [Fact]
    public void Webhook_Should_Throw_Error_If_Webhook_ToFormContent_Called_Before_Setting_Required_Properties()
    {
      var webhook = new Webhook();

      Assert.Throws<InvalidOperationException>(() => {
        webhook.ToFormContent();
      });
    }

    [Fact]
    public void Webhook_Should_Throw_Error_If_Webhook_ToFormContent_Called_Before_Setting_Atleast_One_Required_Url()
    {
      var webhook = new Webhook();

      webhook.SetTypeId(WebHookType.CLICKED);

      Assert.Throws<InvalidOperationException>(() => {
        webhook.ToFormContent();
      });
    }

    [Fact]
    public void Webhook_Should_Throw_Error_If_Webhook_ToFormContent_Called_Before_Setting__Required_IdType()
    {
      var webhook = new Webhook();

      webhook.AppendUrl(new Uri(@"https://example.com"));

      Assert.Throws<InvalidOperationException>(() => {
        webhook.ToFormContent();
      });
    }

    [Theory]
    [InlineData(WebHookType.CLICKED)]
    public void Webhook_Should_Have_Applied_Properties_That_Are_Set(WebHookType type)
    {
      var webhook = new Webhook();

      webhook.SetTypeId(type);

      Assert.Equal(type, webhook.Type);
    }

    [Theory]
    [InlineData(@"https://sample.example.com")]
    public void Webhook_Should_Have_Not_Empty_Urls_When_Url_Is_Appended(string url)
    {
      var webhook = new Webhook();

      var uri = new Uri(url);

      webhook.AppendUrl(uri);

      Assert.NotEmpty(webhook.Urls);
    }
  }
}
