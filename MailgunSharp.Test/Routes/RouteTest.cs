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
    public void Route_Should_Not_Allow_Two_Or_More_Seperate_Expressions()
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

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    public void Route_Should_Throw_Exception_When_Priority_Set_To_Less_Than_Zero(int priority)
    {
      var route = new Route();

      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        route.SetPriority(priority);
      });
    }

    [Fact]
    public void Route_Should_Throw_Exception_When_Expression_Not_Set()
    {
      var route = new Route();

      Assert.Throws<InvalidOperationException>(() => {
        route
          .SetDescription("Failed attempt")
          .MatchHeader("headerName", new Regex(@"^test$"))
          .ToFormContent();
      });
    }

    [Fact]
    public void Route_Should_Throw_Exception_When_Action_Not_Set()
    {
      var route = new Route();

      Assert.Throws<InvalidOperationException>(() => {
        route
          .SetDescription("Failed attempt")
          .Forward(new Uri("https://forward.example.com"))
          .ToFormContent();
      });
    }
  }
}