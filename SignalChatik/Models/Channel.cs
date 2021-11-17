using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Channel name cannot exceed 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public ChannelTypeId ChannelTypeId { get; set; }
        public List<Channel> ForChannels { get; set; }
        public List<Channel> ConnectedChannels { get; set; }
    }
}
