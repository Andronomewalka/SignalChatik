using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class Room : Channel
    {
        [Required]
        public string Owner { get; set; }
    }
}
