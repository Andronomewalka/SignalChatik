using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class UserRefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
