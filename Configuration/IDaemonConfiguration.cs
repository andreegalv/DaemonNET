namespace DaemonNET.Configuration
{
    public interface IDaemonConfiguration
    {
        string Name { get; }

        string Cron { get; }

        IRouteCallbackDaemon RouteCallback { get; }
    }
}
