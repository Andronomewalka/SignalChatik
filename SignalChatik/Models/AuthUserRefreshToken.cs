using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public partial class AuthUserRefreshToken
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }


        [Required]
        public AuthUser AuthUser { get; set; }
    }
}
