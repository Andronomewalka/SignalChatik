using System;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class ChannelContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
        public string Description { get; set; }
        

        [Required]
        public Channel Channel { get; set; }
        public Guid ChannelId { get; set; }
    }
}
