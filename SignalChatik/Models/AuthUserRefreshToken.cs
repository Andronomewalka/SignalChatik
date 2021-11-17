using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class AuthUserRefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
