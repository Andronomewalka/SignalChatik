using SignalChatik.DTO.User;
using System.Collections.Generic;

namespace SignalChatik.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserDTO Owner { get; set; }
        public IEnumerable<UserDTO> Members { get; set; }
    }
}
