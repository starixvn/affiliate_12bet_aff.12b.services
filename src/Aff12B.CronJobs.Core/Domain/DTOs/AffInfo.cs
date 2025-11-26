namespace Aff12B.CronJobs.Core.Domain.DTOs
{
    public class AffInfo
    {
        private const int SITE_12B_ID = 101;

        public int SiteId { get; set; } = SITE_12B_ID;

        public int AffId { get; set; }

        public string AffCode { get; set; }

        public byte AffCategoryId { get; set; }

        public string ParentAff { get; set; }

        public string UserName { get; set; }

        public int Player_CustId { get; set; }

        public string Player_Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public byte DealVerifyFlags { get; set; }
    }
}
