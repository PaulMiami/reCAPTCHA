namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    public class TestLogger : ILogger, IDisposable
    {
        public List<string> Log { get; } = new List<string>();
        public int ScopeCount { get; set; } = 0;

        public void Dispose()
        {
            // Method intentionally left empty.
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            ScopeCount++;
            return this;
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log.Add(formatter(state, exception));
        }
    }
}
