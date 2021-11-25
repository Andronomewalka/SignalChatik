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
                var users = await context.AuthUsers.Include(cur => cur.RefreshTokens)
                                                   .Include(cur => cur.Roles)
                                                   .ToListAsync();

                AuthUser requestedUser = users.FirstOrDefault(cur => cur.Email == userDTO.Email);
                if (requestedUser == null)
                    return JsonResponse.CreateBad(HttpStatusCode.NotFound, $"User not found");

                string storedHash = requestedUser.Hash;
                string storedSalt = requestedUser.Salt;

                if (userDTO.Hash + storedSalt == storedHash)
                {
                    AuthUserRefreshToken userRefreshToken = new AuthUserRefreshToken()
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
                bool isUserExist = context.AuthUsers.Any(cur => cur.Email == userDTO.Email);
                if (isUserExist)
                    return JsonResponse.CreateBad(HttpStatusCode.Forbidden, $"User already exist");

                string salt = Salt.CreateSalt();
                AuthUser user = new AuthUser()
                {
                    Email = userDTO.Email,
                    Hash = userDTO.Hash + salt,
                    Salt = salt,
                    Roles = new List<AuthUserRole>()
                    {
                        new AuthUserRole()
                        {
                            RoleId = AuthUserRoleId.User
                        }
                    },
                    User = new User()
                    {
                        Channel = new Channel()
                        {
                            Name = userDTO.Email
                        }
                    }
                };

                AuthUserRefreshToken userRefreshToken = new AuthUserRefreshToken()
                {
                    RefreshToken = CreateRefreshToken(user)
                };
                user.RefreshTokens = new List<AuthUserRefreshToken>() { userRefreshToken };
                user.RefreshTokens.Add(userRefreshToken);

                await context.AuthUsers.AddAsync(user);
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
                if (!Request.Cookies.TryGetValue("X-Chatik-Refresh-Token", out string refreshToken))
                    return JsonResponse.CreateBad(9001, $"No refresh token in cookies");

                JwtSecurityToken decodedRefreshToken =
                    JwtHelper.ReadToken(refreshToken,
                        jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters);

                if (decodedRefreshToken == null)
                    return JsonResponse.CreateBad(9001, $"Refresh token is broken or expired");

                string tokenGuid = decodedRefreshToken.Payload["sub"]?.ToString();
                string tokenEmail = decodedRefreshToken.Payload["email"]?.ToString();
                if (string.IsNullOrEmpty(tokenGuid) || string.IsNullOrEmpty(tokenEmail))
                    return JsonResponse.CreateBad(9001, $"Payload is invalid");

                var users = await context.AuthUsers
                                         .Include(cur => cur.RefreshTokens)
                                         .Include(cur => cur.Roles)
                                         .ToListAsync();

                AuthUserRefreshToken userCurrentToken =
                    context.AuthUserRefreshTokens.FirstOrDefault(cur => cur.RefreshToken == refreshToken);

                AuthUser user = users.FirstOrDefault(cur =>
                        cur.Guid.ToString() == decodedRefreshToken.Payload["sub"].ToString() &&
                        cur.Email == decodedRefreshToken.Payload["email"].ToString() &&
                        cur.RefreshTokens.Contains(userCurrentToken));

                if (user == null)
                    return JsonResponse.CreateBad(9001, $"Payload is invalid");

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
                Request.Cookies.TryGetValue("X-Chatik-Refresh-Token", out var refreshToken);
                if (string.IsNullOrEmpty(refreshToken))
                    return JsonResponse.CreateBad(HttpStatusCode.Unauthorized, "No token");

                Response.Cookies.Delete("X-Chatik-Refresh-Token");

                List<AuthUserRefreshToken> userCurrentToken = await context.AuthUserRefreshTokens.ToListAsync();
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
        public async Task<IActionResult> Test()
        {
            object access = Request.Headers["Authorization"][0];
            Request.Cookies.TryGetValue("X-Chatik-Refresh-Token", out var refresh);
            await Task.Delay(200);
            return Ok($"good - {UserId}");
        }

        private JsonResult CreateTokensResponse(AuthUser user, string refreshToken)
        {
            Response.Cookies.Append("X-Chatik-Refresh-Token",
                        refreshToken,
                        new CookieOptions()
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddMinutes(authOptions.Value.RefreshTokenLifetimeMin)
                        });

            return JsonResponse.CreateGood(new SignInResponseDTO()
            {
                AccessToken = CreateAccessToken(user),
            });
        }

        private string CreateAccessToken(AuthUser user) => GenerateJWT(user, authOptions.Value.AccessTokenLifetimeMin);
        private string CreateRefreshToken(AuthUser user) => GenerateJWT(user, authOptions.Value.RefreshTokenLifetimeMin);

        private string GenerateJWT(AuthUser user, int expirationMinutes)
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
                claims.Add(new Claim("role", role.RoleId.ToString()));
            }

            JwtSecurityToken token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
