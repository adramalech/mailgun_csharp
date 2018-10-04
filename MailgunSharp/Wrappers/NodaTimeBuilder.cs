using System;
using System.Collections.Generic;
using System.Linq;
using MailgunSharp.Enums;
using NodaTime;

namespace MailgunSharp.Wrappers
{
  public sealed class NodaTimeBuilder : INodaTimeBuilder
  {
    private readonly IClock clock;
    private Instant now;
    private Queue<Tuple<Duration, MathOperation>> operations;

    public NodaTimeBuilder(IClock clock)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.clock = clock;
    }

    public NodaTimeBuilder(DateTime datetime)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.now = Instant.FromDateTimeUtc(datetime);
    }

    public NodaTimeBuilder(int year, int month, int day, int hour, int minute, int second)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.now = Instant.FromUtc(year, month, day, hour, minute, second);
    }

    public INodaTimeBuilder AddDays(int days)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromDays(days), MathOperation.ADD));

      return this;
    }

    public INodaTimeBuilder SubtractDays(int days)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromDays(days), MathOperation.SUBTRACT));

      return this;
    }

    public INodaTimeBuilder AddHours(int hours)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromHours(hours), MathOperation.ADD));

      return this;
    }

    public INodaTimeBuilder SubtractHours(int hours)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromHours(hours), MathOperation.SUBTRACT));

      return this;
    }

    public INodaTimeBuilder AddMinutes(int minutes)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromMinutes(minutes), MathOperation.ADD));

      return this;
    }

    public INodaTimeBuilder SubtractMinutes(int minutes)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromMinutes(minutes), MathOperation.SUBTRACT));

      return this;
    }

    public INodaTimeBuilder AddSeconds(int seconds)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromSeconds(seconds), MathOperation.ADD));

      return this;
    }

    public INodaTimeBuilder SubtractSeconds(int seconds)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromSeconds(seconds), MathOperation.SUBTRACT));

      return this;
    }

    /// <summary>
    /// Build the Nodatime instance based on order of operations.
    /// </summary>
    /// <returns>System.DateTime value in UTC.</returns>
    public Instant Build()
    {
      //initialize the clock if provided.
      if (this.clock != null)
      {
        this.now = this.clock.GetCurrentInstant();
      }

      //iterate over any operations in order of being applied.
      while (this.operations.Any())
      {
        var operation = this.operations.Dequeue();

        switch (operation.Item2)
        {
          case MathOperation.ADD:
            this.now = this.now.Plus(operation.Item1);
            break;

          case MathOperation.SUBTRACT:
            this.now = this.now.Minus(operation.Item1);
            break;
        }
      }

      //return the datetime in utc.
      return this.now;
    }
  }
}