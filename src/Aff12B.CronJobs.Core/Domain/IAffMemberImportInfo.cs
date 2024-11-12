
namespace Aff12B.CronJobs.Core.Domain
{
    public interface IAffMemberImportInfo
    {
        string SiteName { get; }

        string SettingPrefix { get; }

        string XmlData { get; }
    }
}
