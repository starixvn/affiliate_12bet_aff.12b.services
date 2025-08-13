namespace Aff12B.CronJobs.Host.Settings
{
    public class RedisSetting
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Pass { get; set; }

        public int Database { get; set; }

        public string AppName { get; set; }

        public int KeepAlive { get; set; }
    }
}
