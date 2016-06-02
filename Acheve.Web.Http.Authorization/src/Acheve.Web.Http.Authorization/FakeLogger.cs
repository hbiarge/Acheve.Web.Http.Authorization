using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Acheve.Web.Http.Authorization
{
    public class FakeLogger : ILogger<DefaultAuthorizationService>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoOpDisposable();
        }

        public class NoOpDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
