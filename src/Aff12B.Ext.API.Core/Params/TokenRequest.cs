using System.ComponentModel.DataAnnotations;

namespace Aff12B.Ext.API.Core.Params
{
    public class TokenRequest
    {
        private const int MaxLengthOfUserName = 50;
        private const int MaxLengthOfPassword = 16;

        [MaxLength(MaxLengthOfUserName)]
        [Required]
        public string Username { get; set; }

        [MaxLength(MaxLengthOfPassword)]
        [Required]
        public string Password { get; set; }

        public bool IsShowIpvn { get; set; } = true;

    }
}
