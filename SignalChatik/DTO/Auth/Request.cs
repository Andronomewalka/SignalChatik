using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Auth
{
    public class AuthRequestDTO
    {
        public string Email { get; set; } = "";
        public string Hash { get; set; } = "";
    }
}
