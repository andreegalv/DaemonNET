namespace DaemonNET.Configuration
{
    public class DaemonConfiguration : IDaemonConfiguration
    {
        public DaemonConfiguration()
        {
            this.RouteCallback = new RouteCallbackDaemon();
        }

        public required string Name { get; set; }

        public required string Cron { get; set; }

        public IRouteCallbackDaemon RouteCallback { get; set; }
    }
}
