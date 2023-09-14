using BlogAPI.Core.Dtos;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <remarks>
        /// Registers a new user with the provided information.
        /// </remarks>
        /// <param name="registerDto">The user registration information.</param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> representing the result of the registration.
        /// - If the registration is successful, it returns an HTTP 200 OK response.
        /// - If the registration fails, it returns an HTTP 400 Bad Request response with an error message.
        /// - If any other error occurs during registration, it returns an HTTP 500 Internal Server Error response.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registerResult = await _authService.RegisterAsync(registerDto);

            if (registerResult.IsSucceed)
                return Ok(registerResult);

            return BadRequest(registerResult);
        }

        /// <summary>
        /// Log in with user credentials.
        /// </summary>
        /// <remarks>
        /// Attempts to log in a user using the provided credentials (username and password).
        /// </remarks>
        /// <param name="loginDto">The user login credentials.</param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> representing the result of the login attempt.
        /// - If the login is successful, it returns an HTTP 200 OK response with an authentication token.
        /// - If the login fails due to invalid credentials, it returns an HTTP 400 Bad Request response with an error message.
        /// - If any other error occurs during login, it returns an HTTP 500 Internal Server Error response.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSucceed)
                return Ok(loginResult);

            return BadRequest(loginResult);
        }

    }
}
