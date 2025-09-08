using Serilog.Sinks.File.Archive;

namespace Aff12B.Ext.API.Site.SerialLogHooks
{
    public static class ArchiveHook
    {
        public static ArchiveHooks MyArchiveHooks => new ArchiveHooks();
    }
}
