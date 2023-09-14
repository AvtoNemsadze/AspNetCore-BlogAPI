using BlogAPI.Core.Common;
using BlogAPI.Core.DbContexts;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogAPI.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>Registers a new user with the provided information.</summary>
        /// /// <remarks>
        /// Registers a new user with the provided information. The <paramref name="registerDto"/> should contain user details,
        /// including a unique <paramref name="registerDto.UserName"/>. If a user with the same <paramref name="registerDto.UserName"/>
        /// already exists, the registration will fail, and an error message will be returned.
        /// </remarks>
        /// <param name="registerDto">The data required to register a new user.</param>
        /// <returns>
        /// An AuthServiceResponse object containing registration status and message.
        /// - IsSucceed: True if registration is successful; otherwise, false.
        /// - Message: A message indicating the outcome of the registration attempt.
        /// 
        /// 200 OK: Registration is successful, and a user has been created.
        /// 400 Bad Request: If there is a problem with the registration data (e.g., missing fields).
        /// 500 Internal Server Error: An error occurred during the registration process.
        /// </returns>
        public async Task<AuthServiceResponse> RegisterAsync(RegisterDto registerDto)
        {
            var isExistUser = await _context.Users.FirstOrDefaultAsync(user => user.UserName == registerDto.UserName);

            if (isExistUser != null)
                return new AuthServiceResponse() { IsSucceed = false, Message = "UserName Already Exsist" };

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(registerDto.Password, salt);

            ApplicationUser appUser = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PasswordHash = hashedPassword,
                PasswordSalt = Convert.ToBase64String(salt),
                CreatedDate = DateTime.UtcNow,
            };

            _context.Users.Add(appUser);

            if (await _context.SaveChangesAsync() > 0)
            {
                return new AuthServiceResponse() { IsSucceed = true, Message = "User Created Successfully" };
            }
            else
            {
                return new AuthServiceResponse() { IsSucceed = false, Message = "Failed to save user data." };
            }

        }

        private static string HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32); 

                byte[] hashBytes = new byte[48]; 
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }


        /// <summary>Authenticates a user with the provided credentials.</summary>
        /// <param name="loginDto">The data required to authenticate a user.</param>
        /// <returns>
        /// An AuthServiceResponse object containing authentication status and message.
        /// - IsSucceed: True if authentication is successful; otherwise, false.
        /// - Message: A message indicating the outcome of the authentication attempt.
        ///   If authentication is successful, the message may contain an authentication token.
        /// 
        /// 200 OK: Authentication is successful, and the user is logged in.
        /// 400 Bad Request: If there is a problem with the login data (e.g., invalid credentials).
        /// 500 Internal Server Error: An error occurred during the authentication process.
        /// </returns>
        public async Task<AuthServiceResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName == loginDto.UserName);
            if (user is null)
                return new AuthServiceResponse() { IsSucceed = false, Message = "Invalid Credentials" };

            var hashedPassword = HashPassword(loginDto.Password, Convert.FromBase64String(user.PasswordSalt));

            if (hashedPassword != user.PasswordHash)
            {
                return new AuthServiceResponse() { IsSucceed = false, Message = "Invalid Credentials" };
            }

           
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };


            var token = GenerateNewJsonWebToken(authClaims);

            _context.SaveChanges();

            return new AuthServiceResponse()
            {
                IsSucceed = true,
                Message = token,
            };
        }

        public string GenerateNewJsonWebToken(IEnumerable<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenObject);
        }
    }
}
