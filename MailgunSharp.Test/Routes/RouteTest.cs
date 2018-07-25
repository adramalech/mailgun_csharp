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

      Assert.Equal(string.Empty, route.Description);
      Assert.Equal(string.Empty, route.Expression);
      Assert.Equal(0, route.Priority);
      Assert.Equal(0, route.Actions.Count);
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

      Assert.Equal(description, route.Description);
      Assert.NotNull(route.Expression);
      Assert.Equal(priority, route.Priority);
      Assert.NotEmpty(route.Actions);
    }

    [Theory]
    [InlineData("test", 1, "from", @"^*@postmaster.example.com$", @"https://example.com")]
    public void Route_Should_Allow_Multiple_Actions(string description, int priority, string headerName, string pattern, string url)
    {
      var route = new Route();

      route
        .SetDescription(description)
        .SetPriority(priority)
        .MatchHeader(headerName, new Regex(pattern))
        .Forward(new Uri(url))
        .Stop();

      Assert.Equal(description, route.Description);
      Assert.NotNull(route.Expression);
      Assert.Equal(priority, route.Priority);
      Assert.NotEmpty(route.Actions);
    }
  }
}