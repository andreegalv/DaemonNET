namespace DaemonNET
{
    using DaemonNET.Client;
    using DaemonNET.Configuration;

    public partial class DaemonBackgroundService : BackgroundService
    {
        public DaemonBackgroundService(ILoggerFactory loggerFactory, IDaemonConfiguration daemonConfiguration, IHttpClientFactory httpClientFactory)
        {
            ArgumentNullException.ThrowIfNull(daemonConfiguration);

            this._daemonConfiguration = daemonConfiguration;
            this._logger = loggerFactory.CreateLogger<DaemonBackgroundService>();
            this._cronosPeriodicTimer = new CronosPeriodicTimer(this._daemonConfiguration.Cron);
            this._httpClientFactory = httpClientFactory;
        }

        ~DaemonBackgroundService()
        {
            this.Dispose(false);
        }

        private bool _disposed;

        private readonly ILogger<DaemonBackgroundService> _logger;

        private readonly CronosPeriodicTimer _cronosPeriodicTimer;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IDaemonConfiguration _daemonConfiguration;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Excepción general cuando no es posible saber el error")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogWorkerStartedInfo(this._logger, DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (await this._cronosPeriodicTimer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
                    {
                        DateTimeOffset execution = DateTimeOffset.Now;
                        LogWorkerExecutionInfo(this._logger, execution);

                        var daemonHttpClient = new DaemonHttpClient(this._daemonConfiguration, this._httpClientFactory.CreateClient("DaemonHttpClient"));
                        await daemonHttpClient.SendAsync(execution, stoppingToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    LogWorkerCancelledOperationWarn(this._logger);
                }
                catch (HttpRequestException ex)
                {
                    LogWorkerHttpRequestExceptionError(this._logger, ex);
                }
                catch (Exception ex)
                {
                    LogWorkerUnhandleExceptionError(this._logger, ex);
                    Environment.Exit(1);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            this._cronosPeriodicTimer?.Dispose();
            this._disposed = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
