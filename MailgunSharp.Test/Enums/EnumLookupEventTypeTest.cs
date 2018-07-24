using System;
using Xunit;
using MailgunSharp.Enums;

namespace MailgunSharp.Test.Enums
{
  public class EnumLookupEventTypeTest
  {
    [Fact]
    public void Check_Event_Type_Name_Accepted_Should_Return_String_accepted()
    {
      var type = EventType.ACCEPTED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "accepted");
    }

    [Fact]
    public void Check_Event_Type_Name_Clicked_Should_Return_String_clicked()
    {
      var type = EventType.CLICKED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "clicked");
    }

    [Fact]
    public void Check_Event_Type_Name_Complained_Should_Return_String_complained()
    {
      var type = EventType.COMPLAINED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "complained");
    }

    [Fact]
    public void Check_Event_Type_Name_Delivered_Should_Return_String_delivered()
    {
      var type = EventType.DELIVERED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "delivered");
    }

    [Fact]
    public void Check_Event_Type_Name_Failed_Should_Return_String_failed()
    {
      var type = EventType.FAILED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "failed");
    }

    [Fact]
    public void Check_Event_Type_Name_Opened_Should_Return_String_opened()
    {
      var type = EventType.OPENED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "opened");
    }

    [Fact]
    public void Check_Event_Type_Name_Stored_Should_Return_String_stored()
    {
      var type = EventType.STORED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "stored");
    }

    [Fact]
    public void Check_Event_Type_Name_Unsubscribed_Should_Return_String_unsubscribed()
    {
      var type = EventType.UNSUBSCRIBED;

      var name = EnumLookup.GetEventTypeName(type);

      Assert.True(name == "unsubscribed");
    }
  }
}