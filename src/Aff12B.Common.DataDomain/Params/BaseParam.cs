namespace Aff12B.Common.DataDomain.Params
{
    public class BaseParam
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
