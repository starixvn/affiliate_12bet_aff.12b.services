namespace Aff12B.CronJobs.Core.Domain
{
    public interface IPagingInfo
    {
        int PageIndex { get; }

        int PageSize { get; }
    }
}
