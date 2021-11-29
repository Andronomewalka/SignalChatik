using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalChatik.Models
{
    public enum ChannelType
    {
        User, Room
    }

    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public ChannelType ChannelTypeId { get; set; }


        public User User { get; set; }

        public Room Room { get; set; }

        [Required]
        public ICollection<Message> SendedMessages { get; set; }

        [Required]
        public ICollection<Message> ReceivedMessages { get; set; }

        public ICollection<ConnectedChannel> ForChannels { get; set; }
        public ICollection<ConnectedChannel> ConnectedChannels { get; set; }
    }
}
