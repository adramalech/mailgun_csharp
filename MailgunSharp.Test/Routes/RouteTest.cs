using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Xunit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Routes;

namespace MailgunSharp.Test.Routes
{
  public class RouteTest
  {
    [Fact]
    public void Initialize_Route_Should_Be_Empty()
    {
      var route = new Route();

      Assert.True(route.Description == String.Empty);
      Assert.True(route.Expression == String.Empty);
      Assert.True(route.Priority == 0);
      Assert.True(route.Actions.Count == 0);
    }

    [Fact]
    public void Route_Should_Not_Allow_Two_Or_More_Expressions()
    {
      var route = new Route();

      Assert.Throws<InvalidOperationException>(() =>
      {
        route
          .SetDescription("test")
          .SetPriority(1)
          .MatchRecipient(new Regex(@"^*@gmail.com$"))
          .MatchHeader("from", new Regex(@"^*@postmaster.example.com$"))
          .CatchAll();
      });
    }

    [Theory]
    [InlineData("test", 1, "from", @"^*@postmaster.example.com$")]
    public void Route_Should_Allow_One_Expression(string description, int priority, string headerName, string pattern)
    {
      var route = new Route();

      route
        .SetDescription(description)
        .SetPriority(priority)
        .MatchHeader(headerName, new Regex(pattern))
        .Stop();

      Assert.True(route.Description == description);
      Assert.True(route.Expression != null);
      Assert.True(route.Priority == priority);
      Assert.True(route.Actions.Count == 1);
    }

    [Theory]
    [InlineData("test", 1, "from", @"^*@postmaster.example.com$")]
    public void Route_Should_Allow_Multiple_Actions(string description, int priority, string headerName, string pattern)
    {
      var route = new Route();

      route
        .SetDescription(description)
        .SetPriority(priority)
        .MatchHeader(headerName, new Regex(pattern))
        .Forward(new Uri(@"https://example.com"))
        .Stop();

      Assert.True(route.Description == description);
      Assert.True(route.Expression != null);
      Assert.True(route.Priority == priority);
      Assert.True(route.Actions.Count == 2);
    }

    [Fact]
    public void Route_Should_Allow_Complex_Expression()
    {
      var expressionBuilder = new ExpressionBuilder();

      expressionBuilder
        .And(new MatchHeader("name1", new Regex(@"^application/json$")), new MatchRecipient(new Regex(@"*@test.com")))
        .Or(new MatchHeader("name2", new Regex(@"^regextesterhere$")), new MatchRecipient(new Regex(@"*@helloworld.com")));


      var route = new Route();

      route
        .SetDescription("some test string")
        .SetPriority(1)
        .SetExpression(expressionBuilder.Build())
        ;
    }
  }
}