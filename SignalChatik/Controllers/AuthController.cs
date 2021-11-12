using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalChatik.DTO;
using SignalChatik.Helpers;
using SignalChatik.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalChatik.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ChatikContextController
    {
        private readonly IOptionsMonitor<JwtBearerOptions> jwtOptions;
        private readonly IOptions<AuthOptions> authOptions;
        private Guid UserId => Guid.Parse(User.Claims.Single(cur => cur.Type == ClaimTypes.NameIdentifier).Value);

        public AuthController(IOptionsMonitor<JwtBearerOptions> jwtOptions, IOptions<AuthOptions> authOptions, ChatikContext context) : base(context)
        {
            this.jwtOptions = jwtOptions;
            this.authOptions = authOptions;
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] AuthRequestDTO userDTO)
        {
            try
            {
                var users = await context.Users.Include(cur => cur.Roles)
                                               .Include(cur => cur.RefreshTokens)
                                               .ToListAsync();

                User requestedUser = users.FirstOrDefault(cur => cur.Email == userDTO.Email);
                if (requestedUser == null)
                    return JsonResponse.CreateBad(HttpStatusCode.NotFound, $"User not found");

                string storedHash = requestedUser.Hash;
                string storedSalt = requestedUser.Salt;

                if (userDTO.Hash + storedSalt == storedHash)
                {
                    UserRefreshToken userRefreshToken = new UserRefreshToken()
                    {
                        RefreshToken = CreateRefreshToken(requestedUser)
                    };

                    requestedUser.RefreshTokens.Add(userRefreshToken);
                    await context.SaveChangesAsync();
                    return CreateTokensResponse(requestedUser, userRefreshToken.RefreshToken);
                }

                else
                    return JsonResponse.CreateBad(HttpStatusCode.Unauthorized, "Wrong password");
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] AuthRequestDTO userDTO)
        {
            try
            {
                bool isUserExist = context.Users.Any(cur => cur.Email == userDTO.Email);
                if (isUserExist)
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"User already exist");

                string salt = Salt.CreateSalt();
                User user = new User()
                {
                    Email = userDTO.Email,
                    Guid = Guid.NewGuid(),
                    Hash = userDTO.Hash + salt,
                    Salt = salt,
                    Roles = new List<UserRole>()
                    {
                        new UserRole()
                        {
                            Role = Role.User
                        }
                    }
                };

                UserRefreshToken userRefreshToken = new UserRefreshToken()
                {
                    RefreshToken = CreateRefreshToken(user)
                };

                user.RefreshTokens = new List<UserRefreshToken>()
                {
                    userRefreshToken
                };

                await context.Users.AddAsync(user);
                await context.Roles.AddRangeAsync(user.Roles);
                await context.SaveChangesAsync();
                return CreateTokensResponse(user, userRefreshToken.RefreshToken);
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("X-Refresh-Token", out string refreshToken))
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"No refresh token in cookies");

                JwtSecurityToken decodedRefreshToken = ReadToken(refreshToken);
                if (decodedRefreshToken == null)
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"Refresh token is broken or expired");

                string tokenGuid = decodedRefreshToken.Payload["sub"]?.ToString();
                string tokenEmail = decodedRefreshToken.Payload["email"]?.ToString();
                if (string.IsNullOrEmpty(tokenGuid) || string.IsNullOrEmpty(tokenEmail))
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"Payload is invalid");

                var users = await context.Users
                                         .Include(cur => cur.Roles)
                                         .Include(cur => cur.RefreshTokens)
                                         .ToListAsync();

                UserRefreshToken userCurrentToken = 
                    context.RefreshTokens.FirstOrDefault(cur => cur.RefreshToken == refreshToken);

                User user = users.FirstOrDefault(cur =>
                        cur.Guid.ToString() == decodedRefreshToken.Payload["sub"].ToString() &&
                        cur.Email == decodedRefreshToken.Payload["email"].ToString() &&
                        cur.RefreshTokens.Contains(userCurrentToken));

                if (user == null)
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"Payload is invalid");

                userCurrentToken.RefreshToken = CreateRefreshToken(user);
                await context.SaveChangesAsync();
                return CreateTokensResponse(user, userCurrentToken.RefreshToken);
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("sign-out")]
        public async Task<IActionResult> SignOutMethod()
        {
            try
            {
                Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken);
                if (string.IsNullOrEmpty(refreshToken))
                    return JsonResponse.CreateBad(HttpStatusCode.Unauthorized, "No token");

                List<UserRefreshToken> userCurrentToken = await context.RefreshTokens.ToListAsync();
                userCurrentToken.RemoveAll(cur => cur.RefreshToken == refreshToken);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return JsonResponse.CreateBad(HttpStatusCode.InternalServerError, "Server fucked up");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("test")]
        public IActionResult Test()
        {
            object access = Request.Headers["Authorization"][0];
            Request.Cookies.TryGetValue("X-Refresh-Token", out var refresh);
            return Ok($"good - {UserId}");
        }

        private JsonResult CreateTokensResponse(User user, string refreshToken)
        {
            Response.Cookies.Append("X-Refresh-Token",
                        refreshToken,
                        new CookieOptions()
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddMinutes(authOptions.Value.RefreshTokenLifetime)
                        });

            return JsonResponse.CreateGood(new SignInResponseDTO()
            {
                AccessToken = CreateAccessToken(user),
            });
        }

        private string CreateAccessToken(User user) => GenerateJWT(user, authOptions.Value.AccessTokenLifetime);
        private string CreateRefreshToken(User user) => GenerateJWT(user, authOptions.Value.RefreshTokenLifetime);

        private string GenerateJWT(User user, int expirationMinutes)
        {
            AuthOptions authParams = authOptions.Value;

            SymmetricSecurityKey securityKey = authParams.GetSymmetricSecurityKey();
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Guid.ToString()),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.Role.ToString()));
            }

            JwtSecurityToken token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken ReadToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParams =
                    jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
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
