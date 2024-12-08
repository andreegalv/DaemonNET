namespace DaemonNET
{
    using Cronos;

    /// <summary>
    /// Implementación de un PeriodicTimer con Cronos, para mas información del código <see href="https://stackoverflow.com/a/72739676"/> />
    /// </summary>
    public class CronosPeriodicTimer : IDisposable
    {
        public CronosPeriodicTimer(string expression)
        {
            this._cronExpression = CronExpression.Parse(expression);
        }

        private readonly CronExpression _cronExpression;
        private PeriodicTimer? _activeTimer;
        private bool _disposed;

        ~CronosPeriodicTimer()
        {
            this.Dispose(false);
        }

        public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(this._disposed, this);

            cancellationToken.ThrowIfCancellationRequested();

            PeriodicTimer timer;

            lock (this._cronExpression)
            {
                if (this._activeTimer is not null)
                {
                    throw new InvalidOperationException("One consumer at a time.");
                }

                DateTimeOffset now = DateTimeOffset.Now;
                TimeSpan minDelay = TimeSpan.FromMilliseconds(500);

                DateTimeOffset? next = this._cronExpression.GetNextOccurrence(now + minDelay, TimeZoneInfo.Local);

                if (next is null)
                {
                    return false;
                }

                TimeSpan delay = next.Value - now;
                timer = this._activeTimer = new(delay);
            }

            try
            {
                using (timer)
                {
                    return await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                Volatile.Write(ref this._activeTimer, null);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                PeriodicTimer? activeTimer;

                lock (this._cronExpression)
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    this._disposed = true;

                    activeTimer = this._activeTimer;
                }

                activeTimer?.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
