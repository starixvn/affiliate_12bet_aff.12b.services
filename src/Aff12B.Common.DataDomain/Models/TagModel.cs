namespace Aff12B.Common.DataDomain.Models
{
    public class TagModel
    {
        public int Id { get; set; }

        public string TagName { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public DateTime? UpdateTime { get; set; }

        public byte IsAbnormal { get; set; }
    }
}
