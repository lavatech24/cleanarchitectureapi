using AutoMapper;
using Azure.Core;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public AuthService(IAuthRepository authRepository, IMapper mapper, IConfiguration config)
        {
            this.authRepository = authRepository;
            this.mapper = mapper;
            this.config = config;
        }

        public async Task<ApiUser> CreateUserObj(int RoleId, int? CompanyId, bool superuser = false)
        {
            var clientId = this.GenerateClientId();
            if (superuser)
                clientId = $"admin_{this.RandomStringGenerator(10)}";
            var clientSecret = this.GenerateClientSecret();
            var clientSalt = this.GenerateSalt();

            var user = new ApiUser
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Salt = clientSalt,
                ClientSecretHash = this.HashSecret(clientSecret, clientSalt),
                IsActive = true,
                IsSuperuser = superuser,
                RoleId = RoleId,
                CompanyId = CompanyId,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(config["Tokens:RefreshTokenExpireDays"])),
                CreatedAt = DateTime.UtcNow,
            };
            return user;
        }

        #region Authentication and JWT generation methods
        private string GenerateClientId()
        {
            return this.RandomStringGenerator(16);
        }

        private string GenerateClientSecret()
        {
            return this.RandomStringGenerator(32);
        }

        public (string, string, string) GenerateNewSecret()
        {
            var secret = this.GenerateClientSecret();
            var salt = this.GenerateSalt();
            var hashed = this.HashSecret(secret, salt);
            return (secret, salt, hashed);
        }

        private string GenerateSalt()
        {
            byte[] salt = new byte[256 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private string GenerateRefreshToken()
        { 
            return this.GenerateSalt(); 
        }

        private string RandomStringGenerator(int length)
        {
            var resultStr = "";
            var validChars = "abcdefghijklmnopqrstuvwxyz1234567890";
            using var rngProvider = RandomNumberGenerator.Create();
            while (resultStr.Length < length)
            {
                byte[] bt = new byte[1];
                rngProvider.GetBytes(bt);
                char character = (char)bt[0];
                if (validChars.Contains(character))
                {
                    resultStr += character;
                }
            }
            return resultStr;
        }

        private string GetRandomString(int length)
        {
            return this.RandomStringGenerator(length);
        }

        public string HashSecret(string secret, string salt)
        {
            string hashedPass = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: secret,
                    salt: Convert.FromBase64String(salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8)
                );
            return hashedPass;
        }

        /*
        public bool CheckClientCredentials(ApiUser user, string clientSecret)
        {
            var checkSecret = this.HashSecret(clientSecret, user.Salt);
            if (checkSecret == user.ClientSecretHash)
            {
                return true;
            }
            return false;
        }
        */

        public string GenerateJWT(ApiUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.ClientId),
                new Claim(JwtRegisteredClaimNames.Sub, user.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id.ToString())
            };
            claims.Add(new Claim(ClaimTypes.Role, user.Role?.RoleCode.ToLower() ?? "user"));
            if (user.CompanyId.HasValue)
            {
                claims.Add(new Claim("Company", user.Company.CompanyCode.ToString()));
            }

            //Below code is needed if we have multiple roles
            //List<string> roles = new List<string>();
            //user.Role.ForEach(role =>
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //});

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(config["Tokens:JwtExpireMinutes"]));
            //var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(config["Tokens:JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: config["Tokens:JwtIssuer"],
                audience: config["Tokens:Audience"],
                claims: claims,
                expires: expires,
                notBefore: DateTime.UtcNow,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string ExtractJWTClientId(string authHeader)
        {
            try
            {
                var tokenStr = authHeader.Replace("Bearer ", "");
                return this.ExtractJWTClaimValue(
                    JwtRegisteredClaimNames.NameId,
                    tokenStr
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExtractJWTExpires(string tokenStr)
        {
            return this.ExtractJWTClaimValue(
                "exp",
                tokenStr
            );
        }

        private string ExtractJWTClaimValue(string jwtKey, string tokenStr)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenStr);
            foreach (Claim c in token.Claims)
            {
                if (c.Type == jwtKey)
                {
                    return c.Value;
                }
            }
            return null;
        }
        #endregion

        #region Public methods for API user management
        public async Task<bool> ValidateRefreshToken(ApiUserRefreshTokenDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.RefreshToken))
            {
                throw new ArgumentNullException("ClientId and refresh token are required.");
            }
            var user = await authRepository.GetApiUserByClientIdAsync(new LoginDto { ClientId = request.ClientId});
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid clientId credentials.");
            }
            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired Refresh token.");
            }
            return true;
            //return mapper.Map<ApiUserDto>(user);
        }
        public async Task<TokenResponseDto> GenerateRefreshToken(ApiUserRefreshTokenDto request)
        {
            if (!(await ValidateRefreshToken(request)))
            {
                throw new UnauthorizedAccessException("Invalid clientId or refresh token.");
            }
            var user = await authRepository.GetApiUserByClientIdAsync(new LoginDto { ClientId = request.ClientId });
            var tokenResponse = new TokenResponseDto
            {
                ClientId = user.ClientId,
                AccessToken = GenerateJWT(user),
                RefreshToken = this.GenerateRefreshToken(),
            };

            var userDto = new ApiUserUpdateDto() { ClientId = tokenResponse.ClientId, RefreshToken = tokenResponse.RefreshToken
                , RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(config["Tokens:RefreshTokenExpireDays"]))};
            var userUpdateToken = mapper.Map<ApiUserDto>(await authRepository.UpdateRefreshTokenAsync(userDto));
            if (user == null)
            {
                throw new UnauthorizedAccessException("Failed to update refresh token.");
            }
            return tokenResponse;
        }

        public async Task<TokenResponseDto> GetToken(LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
            {
                throw new ArgumentNullException("ClientId and secret are required");
            }
            if (await ValidateClientCredentials(request))
            {
                var user = await authRepository.GetApiUserByClientIdAsync(request);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid client credentials.");
                }
                var refreshToken = GenerateRefreshToken();
                var userDto = mapper.Map<ApiUserUpdateDto>(user);
                userDto.RefreshToken = refreshToken;
                userDto.RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(config["Tokens:RefreshTokenExpireDays"]));
                user = mapper.Map<ApiUser>(await authRepository.UpdateRefreshTokenAsync(userDto));
                return new TokenResponseDto
                {
                    ClientId = user.ClientId,
                    AccessToken = GenerateJWT(user),
                    //Expiry = Convert.ToInt32(config["Tokens:JwtExpireMinutes"]) * 60,
                    RefreshToken = user.RefreshToken,
                    //RefreshTokenExpiry = user.RefreshTokenExpiry
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }
        }

        public async Task<LoginDto> GenerateNewSecret(LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId))
            {
                throw new ArgumentNullException("ClientId is required");
            }
            var user = await authRepository.GetApiUserByClientIdAsync(request);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }
            var (newSecret, newSalt, newHashedSecret) = GenerateNewSecret();
            var updateDto = new ApiUserUpdateDto
            {
                ClientId = user.ClientId,
                ClientSecret = newSecret,
                Salt = newSalt,
                ClientSecretHash = newHashedSecret,
                IsActive = user.IsActive,
            };
            var result = await authRepository.UpdateClientSecretAsync(updateDto);
            return new LoginDto { ClientId = result.ClientId, ClientSecret = newSecret };
        }

        public async Task<bool> ValidateClientCredentials(LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
            {
                throw new ArgumentNullException("ClientId and secret are required");
            }
            var user = await authRepository.GetApiUserByClientIdAsync(request);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }

            var checkSecret = this.HashSecret(request.ClientSecret, user.Salt);
            if (checkSecret == user.ClientSecretHash)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<ApiUserResponseDto> GetApiUser(LoginDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId))
            {
                throw new ArgumentNullException("ClientId is required");
            }
            var user = await authRepository.GetApiUserByClientIdAsync(request);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }
            return mapper.Map<ApiUserResponseDto>(user);
        }

        public async Task<IEnumerable<ApiUserResponseDto>> GetAllApiUsers()
        {
            var users = await authRepository.GetAllApiUsersAsync();
            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException("No users found.");
            }
            return mapper.Map<IEnumerable<ApiUserResponseDto>>(users);
        }

        #endregion
    }
}