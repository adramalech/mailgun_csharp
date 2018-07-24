using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MailgunSharp.Extensions;

namespace MailgunSharp.Routes
{
  public sealed class ExpressionBuilder : IExpressionBuilder
  {
    private StringBuilder expression;

    public ExpressionBuilder()
    {
      this.expression = new StringBuilder();
    }

    public IExpressionBuilder And(IPattern pattern1, IPattern pattern2)
    {
      if (pattern1 == null)
      {
        throw new ArgumentNullException("First pattern cannot be null or empty!");
      }

      if (pattern2 == null)
      {
        throw new ArgumentNullException("Second pattern cannot be null or empty!");
      }

      if (!this.expression.IsEmpty())
      {
        this.expression.Append(" ");
      }

      this.expression.Append($"{pattern1.ToString()} and {pattern2.ToString()}");

      return this;
    }

    public IExpressionBuilder Or(IPattern pattern1, IPattern pattern2)
    {
      if (pattern1 == null)
      {
        throw new ArgumentNullException("First pattern cannot be null or empty!");
      }

      if (pattern2 == null)
      {
        throw new ArgumentNullException("Second pattern cannot be null or empty!");
      }

      if (!this.expression.IsEmpty())
      {
        this.expression.Append(" ");
      }

      this.expression.Append($"{pattern1.ToString()} or {pattern2.ToString()}");

      return this;
    }

    public string Build()
    {
      return this.expression.ToString();
    }

    public override string ToString()
    {
      return this.expression.ToString();
    }
  }
}