using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SignalChatik.Helpers
{
    public class JwtHelper
    {
        public static JwtSecurityToken ReadToken(string token, TokenValidationParameters validationParams)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validate = handler.ValidateToken(token, validationParams, out SecurityToken securityToken);
                return securityToken as JwtSecurityToken;
            }
            catch (SecurityTokenException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
