using SignalChatik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Channel
{
    public class ChannelDTO
    {
        public int Id { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
