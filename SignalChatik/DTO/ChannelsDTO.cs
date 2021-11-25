using SignalChatik.Models;
using System.Collections.Generic;

namespace SignalChatik.DTO
{
    public class ChannelDTO
    {
        public int Id { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class GetChannelsDTO
    {
        public IEnumerable<ChannelDTO> Channels { get; set; }
    }

    public class ConnectChannelDTO
    {
        public ChannelDTO Channel { get; set; }
    }
}
