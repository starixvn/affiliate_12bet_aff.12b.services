namespace Aff12B.CronJobs.Infrastructure.Helpers
{
    public static class PagingHelper
    {
        public static IEnumerable<IEnumerable<T>> PagingData<T>(IEnumerable<T> datas, int max)
        {
            if (max <= 0)
            {
                throw new ArgumentException("Page size must be greater than zero", nameof(max));
            }

            if (datas == null)
            {
                throw new ArgumentNullException(nameof(datas), "datas cannot be null");
            }

            var result = new List<IEnumerable<T>>
            {
                datas.Take(max)
            };
            var remain = datas.Skip(max);
            while (remain.Any())
            {
                result.Add(remain.Take(max));
                remain = remain.Skip(max);
            }
            return result;
        }
    }
}
