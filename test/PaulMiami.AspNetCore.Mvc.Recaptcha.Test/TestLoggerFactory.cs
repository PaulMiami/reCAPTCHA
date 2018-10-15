namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    using Microsoft.Extensions.Logging;

    public class TestLoggerFactory<T> : ILoggerFactory
    {
        private readonly ILogger _logger;

        public TestLoggerFactory(ILogger logger)
        {
            _logger = logger;
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
            throw new System.NotImplementedException();
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return _logger;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void System.IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
