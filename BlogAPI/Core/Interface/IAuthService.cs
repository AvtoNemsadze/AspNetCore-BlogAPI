using BlogAPI.Core.Common;
using BlogAPI.Core.Dtos;

namespace BlogAPI.Core.Interface
{
    public interface IAuthService
    {
        Task<AuthServiceResponse> RegisterAsync(RegisterDto registerDto);
        Task<AuthServiceResponse> LoginAsync(LoginDto loginDto);
    }
}
