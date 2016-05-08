using System;
using Microsoft.AspNet.Authorization;
using Microsoft.Extensions.Logging;

namespace Acheve.Web.Http.Authorization
{
    public class FakeLogger : ILogger<DefaultAuthorizationService>
    {
        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScopeImpl(object state)
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
