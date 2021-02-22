using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Logging
{
    public class FakeLogger<T> : ILogger<T>
    {
        public string LogMessage { get; private set; }
        public object LogScope { get; private set; }
        public LogLevel LogLevel { get; private set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            LogScope = state;
            return new Mock<IDisposable>().Object;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogMessage = state.ToString();
            LogLevel = logLevel;
        }
    }

    public class FakeBadLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new Exception();
        }
    }
}
