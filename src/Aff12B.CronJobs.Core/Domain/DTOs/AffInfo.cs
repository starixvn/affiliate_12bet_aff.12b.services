namespace Aff12B.CronJobs.Core.Domain.DTOs
{
    public class AffInfo
    {
        public string AffCode { get; set; }

        public string ParentAff { get; set; }

        public string UserName { get; set; }

        public int Player_CustId { get; set; }

        public string Player_Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }
    }
}
