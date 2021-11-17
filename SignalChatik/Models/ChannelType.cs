using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Models
{
    public enum ChannelTypeId
    {
        User, Room
    }

    public class ChannelType
    {
        [Key]
        public ChannelTypeId Id { get; set; }

        public string Name { get; set; }

        public List<Channel> Channels { get; set; }
    }
}
