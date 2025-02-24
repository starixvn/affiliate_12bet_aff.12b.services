namespace Aff12B.CronJobs.Infrastructure.Comparer
{
    public class StringToIntComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xInt = Convert.ToInt32(x);
            var yInt = Convert.ToInt32(y);
            return xInt - yInt;
        }
    }
}
