using System;
using System.Text.RegularExpressions;
using Xunit;
using MailgunSharp.Routes;

namespace MailgunSharp.Test.Routes
{
  public class RouteTest
  {
    [Fact]
    public void Initialize_Route_Should_Be_Empty()
    {
      var route = new Route();

      Assert.True(route.Description == string.Empty);
      Assert.True(route.Expression == string.Empty);
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
          .MatchRecipient(new Regex(@"^@gmail.com$"))
          .MatchHeader("from", new Regex(@"^@postmaster.example.com$"))
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
  }
}