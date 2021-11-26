using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Room
{
    public class RoomResponseDTO : RoomRequestDTO
    {
        public RoomDTO Room { get; set; }
    }
}
