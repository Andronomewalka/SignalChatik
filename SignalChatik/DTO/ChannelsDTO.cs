using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO
{
    public enum ChannelType
    {
        User, Room
    }

    public class ChannelDTO
    {
        public int Id { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChannelRoomDTO : ChannelDTO
    {
        public ChannelDTO OwnerUser { get; set; }
        public IEnumerable<ChannelDTO> Members { get; set; }
    }

    public class ChannelsDTO
    {
        public List<ChannelDTO> Channels { get; set; }
    }
}
