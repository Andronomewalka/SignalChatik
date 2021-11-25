using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public Channel Channel { get; set; }

        [Required]
        public int ChannelId { get; set; }

        [Required]
        public User Owner { get; set; }

        [Required]
        public int OwnerId { get; set; }
    }
}
