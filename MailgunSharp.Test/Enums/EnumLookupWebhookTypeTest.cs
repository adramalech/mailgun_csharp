using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupWebhookTypeTest
  {
    [Fact]
    public void Check_Webhook_Type_Name_Clicked_Should_Return_String_clicked()
    {
      var type = WebHookType.CLICKED;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "clicked");
    }

    [Fact]
    public void Check_Webhook_Type_Name_Complained_Should_Return_String_complained()
    {
      var type = WebHookType.COMPLAINED;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "complained");
    }

    [Fact]
    public void Check_Webhook_Type_Name_Delivered_Should_Return_String_delivered()
    {
      var type = WebHookType.DELIVERED;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "delivered");
    }

    [Fact]
    public void Check_Webhook_Type_Name_Opened_Should_Return_String_opened()
    {
      var type = WebHookType.OPENED;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "opened");
    }

    [Fact]
    public void Check_Webhook_Type_Name_PermanentFailed_Should_Return_String_permanent_failed()
    {
      var type = WebHookType.PERMANENT_FAIL;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "permanent_fail");
    }

    [Fact]
    public void Check_Webhook_Type_Name_TemporaryFail_Should_Return_String_temporary_fail()
    {
      var type = WebHookType.TEMPORARY_FAIL;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "temporary_fail");
    }

    [Fact]
    public void Check_Webhook_Type_Name_Unsubscribed_Should_Return_String_unsubscribed()
    {
      var type = WebHookType.UNSUBSCRIBED;

      var name = EnumLookup.GetWebhookTypeName(type);

      Assert.True(name == "unsubscribed");
    }
  }
}