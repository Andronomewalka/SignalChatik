using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SignalChatik.Helpers
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int AccessTokenLifetimeMin { get; set; }
        public int RefreshTokenLifetimeMin { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}
