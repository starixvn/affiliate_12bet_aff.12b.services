using Serilog.Sinks.File.Archive;
using System.IO.Compression;

namespace Aff12B.Ext.API.Site.SerialLogHooks
{
    public static class ArchiveHook
    {
        public static ArchiveHooks MyArchiveHooks => new ArchiveHooks(CompressionLevel.Fastest);
    }
}
