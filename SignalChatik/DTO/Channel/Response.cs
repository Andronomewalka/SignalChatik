using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Channel
{
    public class GetChannelsResponseDTO
    {
        public IEnumerable<ChannelDTO> Channels { get; set; }
    }

    public class ConnectChannelResponseDTO
    {
        public ChannelDTO Channel { get; set; }
    }
}
