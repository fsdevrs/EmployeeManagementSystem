using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GenralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
    }
}
