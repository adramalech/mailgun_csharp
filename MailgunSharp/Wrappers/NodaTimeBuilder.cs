using System;
using System.Collections.Generic;
using System.Linq;
using MailgunSharp.Enums;
using NodaTime;

namespace MailgunSharp.Wrappers
{
  public sealed class NodaTimeBuilder : INodaTimeBuilder
  {
    /// <summary>
    /// The instance of the clock to derive the instance of time from.
    /// </summary>
    private readonly IClock clock;

    /// <summary>
    /// The current time of now.
    /// </summary>
    private Instant now;

    /// <summary>
    /// The queue of operations to be applied in order of called.
    /// </summary>
    private Queue<Tuple<Duration, MathOperation>> operations;

    /// <summary>
    /// Constructor that uses a injected system clock to be used in generating the instant.
    /// </summary>
    /// <param name="clock">The system clock be used.</param>
    public NodaTimeBuilder(IClock clock)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.clock = clock;
    }

    /// <summary>
    /// Pass in a datetime that is UTC.
    /// </summary>
    /// <param name="datetime">A datetime in UTC.</param>
    public NodaTimeBuilder(DateTime datetime)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.now = Instant.FromDateTimeUtc(datetime);
    }

    /// <summary>
    /// Generate an instance of the builder with a custom date.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    public NodaTimeBuilder(int year, int month, int day, int hour, int minute, int second)
    {
      this.operations = new Queue<Tuple<Duration, MathOperation>>();

      this.now = Instant.FromUtc(year, month, day, hour, minute, second);
    }

    /// <summary>
    /// Add N number of days.
    /// </summary>
    /// <param name="days">The value of days to be added.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder AddDays(int days)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromDays(days), MathOperation.ADD));

      return this;
    }

    /// <summary>
    /// Subtract N number of days.
    /// </summary>
    /// <param name="days">The days to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder SubtractDays(int days)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromDays(days), MathOperation.SUBTRACT));

      return this;
    }

    /// <summary>
    /// Add N number of hours.
    /// </summary>
    /// <param name="hours">The hours to be added.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder AddHours(int hours)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromHours(hours), MathOperation.ADD));

      return this;
    }

    /// <summary>
    /// Subtract N number of hours.
    /// </summary>
    /// <param name="hours">The hours to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder SubtractHours(int hours)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromHours(hours), MathOperation.SUBTRACT));

      return this;
    }

    /// <summary>
    /// Add N number of minutes.
    /// </summary>
    /// <param name="minutes">The minutes to be added.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder AddMinutes(int minutes)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromMinutes(minutes), MathOperation.ADD));

      return this;
    }

    /// <summary>
    /// Subtract N number of minutes.
    /// </summary>
    /// <param name="minutes">The minutes to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder SubtractMinutes(int minutes)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromMinutes(minutes), MathOperation.SUBTRACT));

      return this;
    }

    /// <summary>
    /// Add N number of seconds.
    /// </summary>
    /// <param name="seconds">The seconds to be added.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder AddSeconds(int seconds)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromSeconds(seconds), MathOperation.ADD));

      return this;
    }

    /// <summary>
    /// Subtract N number of seconds.
    /// </summary>
    /// <param name="seconds">The seconds to be subtracted.</param>
    /// <returns>The instance of the builder.</returns>
    public INodaTimeBuilder SubtractSeconds(int seconds)
    {
      this.operations.Enqueue(new Tuple<Duration, MathOperation>(Duration.FromSeconds(seconds), MathOperation.SUBTRACT));

      return this;
    }

    /// <summary>
    /// Build the NodaTime instance based on order of operations.
    /// </summary>
    /// <returns>The resulting NodaTime instant which represents a datetime value in UTC.</returns>
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

      return this.now;
    }
  }
}