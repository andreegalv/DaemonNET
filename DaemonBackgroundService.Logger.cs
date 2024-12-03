namespace DaemonNET
{
    public partial class DaemonBackgroundService
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Worker initialized at {Time}")]
        private static partial void LogWorkerStartedInfo(ILogger logger, DateTimeOffset Time);

        [LoggerMessage(Level = LogLevel.Information, Message = "Worker running at: {Time}")]
        private static partial void LogWorkerExecutionInfo(ILogger logger, DateTimeOffset Time);

        [LoggerMessage(Level = LogLevel.Error, Message = "An unexpected error happen")]
        private static partial void LogWorkerUnhandleExceptionError(ILogger logger, Exception? ex);

        [LoggerMessage(Level = LogLevel.Error, Message = "An exception occurs when using HttpClient")]
        private static partial void LogWorkerHttpRequestExceptionError(ILogger logger, Exception? ex);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Worker operation cancelled")]
        private static partial void LogWorkerCancelledOperationWarn(ILogger logger);
    }
}
