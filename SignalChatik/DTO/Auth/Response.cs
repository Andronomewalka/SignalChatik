using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.DTO.Auth
{
    public class SignInResponseDTO
    {
        public string AccessToken { get; set; }
    }

    public class SignUpResponseDTO
    {
        public string AccessToken { get; set; }
        public string ServerValidationError { get; set; }
    }
}
